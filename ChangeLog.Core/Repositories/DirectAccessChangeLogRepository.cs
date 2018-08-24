using System.Collections.Generic;
using System.Linq;
using ChangeLog.Core.Models;
using ChangeLog.Core.Services;
using SemVer;
using System.Threading.Tasks;
using System;

namespace ChangeLog.Core.Repositories
{
	public class DirectAccessChangeLogRepository : IChangeLogRepository, IInitializeable
	{
		private readonly List<ChangeLogCommit> _commits = new List<ChangeLogCommit>();
		private readonly IGithubService _githubService;
		private readonly IGitService _gitService;
		private bool _initialized = false;

		public DirectAccessChangeLogRepository(IGithubService githubService, IGitService gitService)
		{
			_githubService = githubService;
			_gitService = gitService;
		}

		public async Task InitializeAsync()
		{
			var prs = await _githubService.GetPullRequests();
			_commits.AddRange(_gitService.GetChangeLogCommits(prs));
			_initialized = true;
		}

		public async Task<List<ChangeLogCommit>> GetAllCommitsAsync()
		{
			if (!_initialized)
			{
				throw new InvalidOperationException($"{nameof(DirectAccessChangeLogRepository)} has not been initialized");
			}
			return await Task.FromResult(_commits);
		}

		public async Task<List<SemVer.Version>> GetAllVersionsAsync()
		{
			if (!_initialized)
			{
				throw new InvalidOperationException($"{nameof(DirectAccessChangeLogRepository)} has not been initialized");
			}
			return await Task.FromResult(_commits.SelectMany(c => c.Versions).Distinct().ToList());
		}

		public Task PersistAsync(List<ChangeLogCommit> commits)
		{
			throw new System.NotImplementedException();
		}
	}
}