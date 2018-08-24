using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeLog.Core.Models;
using ChangeLog.Core.Repositories;
using ChangeLog.Core.Services;
using ChangeLog.Core.Settings;
using McMaster.Extensions.CommandLineUtils;
using McMaster.Extensions.CommandLineUtils.Validation;
using System.ComponentModel.DataAnnotations;

public class ShowCommand
{
	private readonly CosmosDbRepository _cosmosDbRepository;
	private readonly DirectAccessChangeLogRepository _directAccessChangeLogRepository;

	public ShowCommand(CosmosDbRepository cosmoDbRepository, DirectAccessChangeLogRepository directAccessChangeLogRepository)
	{
		_cosmosDbRepository = cosmoDbRepository;
		_directAccessChangeLogRepository = directAccessChangeLogRepository;
	}

	public void Configure(CommandLineApplication showCommand)
	{
		showCommand.Description = "Show a change log between ";
		var currentVersionArg = showCommand.Option(
				"-c|--CurrentVersion <CurrentVersion>",
				"The version you are currently on",
				CommandOptionType.SingleValue)
			.IsRequired();
		currentVersionArg.Validators.Add(new MustBeSemanticVersionValidator());

		var targetVersionArg = showCommand.Option(
				"-t|--TargetVersion <TargetVersion>",
				"The version you wish to see a changelog for",
				CommandOptionType.SingleValue)
			.IsRequired();
		targetVersionArg.Validators.Add(new MustBeSemanticVersionValidator());

		var sourceArg = showCommand.Option(
			"-s|--dataSource <DataSource>",
			"The datasource to query. Possible values: Github|Cosmos",
			CommandOptionType.SingleValue);
		sourceArg.Accepts().Values("Github", "Cosmos");

		showCommand.OnExecute(async () =>
		{
			var currentVersion = currentVersionArg.Value();
			var targetVersion = targetVersionArg.Value();
			IChangeLogRepository repositoryToUse = _cosmosDbRepository;
			if (string.Equals(sourceArg.Value(), "github", StringComparison.OrdinalIgnoreCase))
			{
				repositoryToUse = _directAccessChangeLogRepository;
			}
			await RunAsync(repositoryToUse, currentVersion, targetVersion);
			return 0;
		});
	}

	public async Task RunAsync(
		IChangeLogRepository repository,
		string currentVersion,
		string targetVersion)
	{
		var initializable = repository as IInitializeable;
		if (initializable != null)
		{
			await initializable.InitializeAsync();
		}

		var changeLogCalculator = new ChangeLogCalculatorService(repository);

		var changeLogs = await changeLogCalculator.CalculateChangeLogAsync(currentVersion, targetVersion);
		foreach (var log in changeLogs)
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