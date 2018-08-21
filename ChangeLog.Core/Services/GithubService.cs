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

namespace ChangeLog.Core.Services
{

	interface IGithubService
	{
		Task<List<PullRequest>> GetPullRequests();
	}

	public class GithubService : IGithubService
	{

		private HttpClient _client;
		private DataContractJsonSerializer _serializer;
		private readonly GithubSettings _settings;

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
			var allPullRequests = new List<PullRequest>();
			for (int page = 0; page < 3; page++)
			{
				var pullRequestsFromPage = await GetPullRequests(page);
				allPullRequests.AddRange(pullRequestsFromPage);
			}
			return allPullRequests;
		}

		private async Task<List<PullRequest>> GetPullRequests(int page)
		{
			var url = $"/repos/{_settings.GithubProjectPath}/pulls?state=closed&per_page=100&page={page}";
			var response = await _client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
			response.EnsureSuccessStatusCode();
			var content = await response.Content.ReadAsStringAsync();
			try
			{
				return _serializer.ReadObject(await response.Content.ReadAsStreamAsync()) as List<PullRequest>;
			}
			catch (SerializationException ex)
			{
				throw;
			}

		}

	}
}