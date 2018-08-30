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
using Microsoft.Azure.Documents.Linq;

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
				var feedOptions = new FeedOptions() { MaxItemCount = 30 };
				const string query = "SELECT * FROM PullRequests p ORDER BY p.ChangeLogCommit.PullRequest.merged_at DESC";
				var documentUri = UriFactory.CreateDocumentCollectionUri(database.Id, collection.Id);
				var changeLogCommitDocuments = new List<ChangeLogCommitDocument>();

				var documentQuery = client.CreateDocumentQuery<ChangeLogCommitDocument>(documentUri, query, feedOptions).AsDocumentQuery();

				while (documentQuery.HasMoreResults)
				{
					var documents = await documentQuery.ExecuteNextAsync<ChangeLogCommitDocument>();
					changeLogCommitDocuments.AddRange(documents);
				}

				return changeLogCommitDocuments.Select(d => d.ChangeLogCommit).ToList();
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