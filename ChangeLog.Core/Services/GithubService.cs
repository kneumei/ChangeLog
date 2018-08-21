using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
			var response = await _client.GetAsync($"/repos/{_settings.GithubProjectPath}/pulls?state=closed", HttpCompletionOption.ResponseHeadersRead);
			response.EnsureSuccessStatusCode();
			return _serializer.ReadObject(await response.Content.ReadAsStreamAsync()) as List<PullRequest>;
		}

	}
}