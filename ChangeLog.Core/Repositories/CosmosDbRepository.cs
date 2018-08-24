using System.Collections.Generic;
using ChangeLog.Core.Models;
using SemVer;
using Microsoft.Azure.Documents.Client;
using ChangeLog.Core.Settings;
using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Azure.Documents;
using Microsoft.Extensions.Options;

namespace ChangeLog.Core.Repositories
{
	public class CosmosDbRepository : IChangeLogRepository
	{

		public CosmosDbSettings _settings;


		public CosmosDbRepository(IOptions<CosmosDbSettings> settings) : this(settings.Value)
		{
		}
		public CosmosDbRepository(CosmosDbSettings settings)
		{
			_settings = settings;
		}

		public async Task<List<ChangeLogCommit>> GetAllCommitsAsync()
		{
			using (var client = new DocumentClient(new Uri(_settings.CosmosDbUri), _settings.CosmosDbKey))
			{
				var database = GetDatabase(client);
				var collection = GetCollection(client, database);
				var requestOptions = new RequestOptions();

				var docs = await client.ReadDocumentFeedAsync(collection.DocumentsLink, new FeedOptions() { MaxItemCount = 400 });

				return docs
					.Select(d => ((ChangeLogCommitDocument)d))
					.Select(d => d.ChangeLogCommit)
					.ToList();
			}
		}

		public async Task<List<SemVer.Version>> GetAllVersionsAsync()
		{
			var allCommits = await GetAllCommitsAsync();
			return allCommits
				.SelectMany(c => c.Versions)
				.Distinct()
				.ToList();

		}

		public async Task PersistAsync(List<ChangeLogCommit> commits)
		{
			using (var client = new DocumentClient(new Uri(_settings.CosmosDbUri), _settings.CosmosDbKey))
			{
				var database = GetDatabase(client);
				var collection = GetCollection(client, database);
				var requestOptions = new RequestOptions();

				foreach (var commit in commits)
				{
					await client.UpsertDocumentAsync(
						collection.DocumentsLink,
						new ChangeLogCommitDocument(commit),
						requestOptions,
						disableAutomaticIdGeneration: true);
				}
			}
		}

		private Database GetDatabase(DocumentClient client)
		{
			return client.CreateDatabaseQuery()
				.Where(db => db.Id == _settings.CosmosDbDatabaseId)
				.ToArray()
				.First();
		}

		private DocumentCollection GetCollection(DocumentClient client, Database database)
		{
			var databaseUri = UriFactory.CreateDatabaseUri(database.Id);

			return client.CreateDocumentCollectionQuery(databaseUri)
				.Where(c => c.Id == "PullRequests")
				.AsEnumerable()
				.First();

		}
	}
}