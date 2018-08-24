using System.Collections.Generic;
using System.Linq;
using ChangeLog.Core.Models;
using ChangeLog.Core.Services;
using SemVer;
using System.Threading.Tasks;
using System;

namespace ChangeLog.Core.Repositories
{
	public class DirectAccessChangeLogRepository : IChangeLogRepository
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

		public async Task Initialize()
		{
			var prs = await _githubService.GetPullRequests();
			_commits.AddRange(_gitService.GetChangeLogCommits(prs));
			_initialized = true;
		}

		public List<ChangeLogCommit> GetAllCommits()
		{
			if (!_initialized)
			{
				throw new InvalidOperationException($"{nameof(DirectAccessChangeLogRepository)} has not been initialized");
			}
			return _commits;
		}

		public List<SemVer.Version> GetAllVersions()
		{
			if (!_initialized)
			{
				throw new InvalidOperationException($"{nameof(DirectAccessChangeLogRepository)} has not been initialized");
			}
			return _commits.SelectMany(c => c.Tags).Distinct().ToList();
		}

		public Task PersistAsync(List<ChangeLogCommit> commits)
		{
			throw new System.NotImplementedException();
		}
	}
}