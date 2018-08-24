using System.Collections.Generic;
using ChangeLog.Core.Models;
using SemVer;
using Microsoft.Azure.Documents.Client;
using ChangeLog.Core.Settings;
using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Azure.Documents;

namespace ChangeLog.Core.Repositories
{
	public class CosmosDbRepository : IChangeLogRepository
	{

		public CosmosDbSettings _settings;


		public CosmosDbRepository(CosmosDbSettings settings)
		{
			_settings = settings;
		}

		public Task<List<ChangeLogCommit>> GetAllCommitsAsync()
		{
			throw new System.NotImplementedException();
		}

		public Task<List<SemVer.Version>> GetAllVersionsAsync()
		{
			throw new System.NotImplementedException();
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