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

			var gitHubService = new GithubService(githubSettings);
			var gitService = new GitService(gitSettings);
			var cosmoDbRepository = new CosmosDbRepository(cosmosDbSettings);
			var directAccessRepository = new DirectAccessChangeLogRepository(gitHubService, gitService);

			var app = new CommandLineApplication();
			app.HelpOption(inherited: true);
			app.Command("load", loadCommand =>
			{
				loadCommand.Description = "Load Pull Requests into Data Store";
			});

			var showCommand = new ShowCommand(cosmoDbRepository, directAccessRepository);

			app.Command("show", showCommand.Configure);


			app.OnExecute(() =>
			{
				Console.WriteLine("Specify a subcommand");
				app.ShowHelp();
				return 1;
			});

			return app.Execute(args);
		}
	}
}
