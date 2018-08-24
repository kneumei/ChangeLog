using System;
using System.Threading.Tasks;
using ChangeLog.Core.Services;
using ChangeLog.Core.Repositories;
using ChangeLog.Core.Models;
using SemVer;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.IO;
using ChangeLog.Core.Settings;
using McMaster.Extensions.CommandLineUtils;
using McMaster.Extensions.CommandLineUtils.Validation;
using System.ComponentModel.DataAnnotations;

namespace ChangeLog.CLI
{
	class Program
	{

		static int Main(string[] args)
		{
			var configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
				.AddEnvironmentVariables("ChangeLog:")
				.AddCommandLine(args)
				.Build();

			var githubSettings = new GithubSettings();
			configuration.Bind(githubSettings);

			var gitSettings = new GitSettings();
			configuration.Bind(gitSettings);

			var cosmosDbSettings = new CosmosDbSettings();
			configuration.Bind(cosmosDbSettings);

			var app = new CommandLineApplication();
			app.HelpOption();
			var beginningVersionArg = app.Option(
					"-b|--beginningVersion <BeginningVersion>",
					"The version you are currently on",
					CommandOptionType.SingleValue)
				.IsRequired();
			beginningVersionArg.Validators.Add(new MustBeSemanticVersionValidator());

			var endingVersionArg = app.Option(
					"-t|--targetVersion <TargetVersion>",
					"The version you wish to see a changelog for",
					CommandOptionType.SingleValue)
				.IsRequired();
			endingVersionArg.Validators.Add(new MustBeSemanticVersionValidator());

			app.OnExecute(async () =>
			{
				var beginningVersion = beginningVersionArg.Value();
				var endingVersion = endingVersionArg.Value();
				await RunAsync(githubSettings, gitSettings, cosmosDbSettings, beginningVersion, endingVersion);
				return 0;
			});

			return app.Execute(args);
		}

		private static async Task RunAsync(
			GithubSettings githubSettings,
			GitSettings gitSettings,
			CosmosDbSettings cosmosDbSettings,
			string beginningVersion,
			string endingVersion)
		{

			var githubService = new GithubService(githubSettings);
			var gitService = new GitService(gitSettings);
			var repository = new DirectAccessChangeLogRepository(githubService, gitService);
			var cosmos = new CosmosDbRepository(cosmosDbSettings);

			// await repository.Initialize();

			// var changeLogCalculator = new ChangeLogCalculatorService(repository);

			// var changeLogResults = await changeLogCalculator.CalculateChangeLogAsync(beginningVersion, endingVersion);
			// var changeLogs = new List<ChangeLogCommit>() { changeLogResults.First() };
			// foreach (var log in changeLogs)
			// {
			// 	Console.WriteLine($"{log.PullRequest.Title} ({log.Versions.Min()})");

			// }
			// await cosmos.PersistAsync(changeLogs);

			var allCommits = await cosmos.GetAllCommitsAsync();
			foreach (var log in allCommits)
			{
				Console.WriteLine($"{log.PullRequest.Title} ({log.Versions.Min()})");
			}
		}

		class MustBeSemanticVersionValidator : IOptionValidator
		{
			public ValidationResult GetValidationResult(CommandOption option, ValidationContext context)
			{
				if (!option.HasValue()) return ValidationResult.Success;
				var value = option.Value();

				try
				{
					new SemVer.Version(value);
				}
				catch (ArgumentException)
				{
					return new ValidationResult($"The value for --{option.LongName} must be a valid Semantic Version");
				}
				return ValidationResult.Success;
			}
		}
	}
}
