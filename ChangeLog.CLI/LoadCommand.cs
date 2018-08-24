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

public class LoadCommand
{
	private readonly CosmosDbRepository _cosmosDbRepository;
	private readonly DirectAccessChangeLogRepository _directAccessRepository;

	public LoadCommand(CosmosDbRepository cosmoDbRepository, DirectAccessChangeLogRepository directAccessRepository)
	{
		_cosmosDbRepository = cosmoDbRepository;
		_directAccessRepository = directAccessRepository;
	}

	public void Configure(CommandLineApplication loadCommand)
	{
		loadCommand.Description = "Load pull requests into Cosmos";
		var lookbackOptionName = nameof(GithubSettings.GithubPullRequestLookbackNumber);
		var lookback = loadCommand.Option<int>(
				$"--{lookbackOptionName} <{lookbackOptionName}>",
				"The number of pull requests to download from Github.",
				CommandOptionType.SingleValue)
			.Accepts(o => o.Range(0, 1000))
			.IsRequired();

		loadCommand.OnExecute(async () =>
		{
			await _directAccessRepository.InitializeAsync();
			var commits = await _directAccessRepository.GetAllCommitsAsync();
			await _cosmosDbRepository.PersistAsync(commits);
			Console.WriteLine($"Inserted or Updated {commits.Count} commits");
		});
	}

}