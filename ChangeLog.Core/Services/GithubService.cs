using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using ChangeLog.Core.Models;
using ChangeLog.Core.Settings;
using Microsoft.Extensions.Options;

namespace ChangeLog.Core.Services
{

	public interface IGithubService
	{
		Task<List<PullRequest>> GetPullRequests();
	}

	public class GithubService : IGithubService
	{

		private HttpClient _client;
		private DataContractJsonSerializer _serializer;
		private readonly GithubSettings _settings;

		public GithubService(IOptions<GithubSettings> settings) : this(settings.Value)
		{
		}

		public GithubService(GithubSettings settings)
		{
			_settings = settings;
			_client = new HttpClient();
			_client.BaseAddress = new Uri("https://api.github.com");
			_client.DefaultRequestHeaders.Accept.Add(
				new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json")
			);
			_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", _settings.GithubApiToken);
			_client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("ReleaseNotes", "0.0.1"));

			_serializer = new DataContractJsonSerializer(typeof(List<PullRequest>));
		}

		public async Task<List<PullRequest>> GetPullRequests()
		{

			int lookbackNumber = Math.Max(1, _settings.GithubPullRequestLookbackNumber);
			int requestsPerPage = 50;
			int numberRequests = (int)Math.Ceiling((decimal)lookbackNumber / (decimal)requestsPerPage);

			var allPullRequests = new List<PullRequest>();
			for (int page = 0; page < numberRequests; page++)
			{
				var previouscount = page * requestsPerPage;
				var perPage = Math.Min(requestsPerPage, lookbackNumber - previouscount);
				var pullRequestsFromPage = await GetPullRequests(page, perPage);
				allPullRequests.AddRange(pullRequestsFromPage);
			}
			return allPullRequests;
		}

		private async Task<List<PullRequest>> GetPullRequests(int page, int perPage)
		{
			var url = $"/repos/{_settings.GithubProjectPath}/pulls?state=closed&per_page={perPage}&page={page}";
			var response = await _client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
			response.EnsureSuccessStatusCode();
			var content = await response.Content.ReadAsStringAsync();
			try
			{
				return _serializer.ReadObject(await response.Content.ReadAsStreamAsync()) as List<PullRequest>;
			}
			catch (SerializationException)
			{
				throw;
			}

		}

	}
}