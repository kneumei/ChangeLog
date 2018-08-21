using System.Collections.Generic;
using System.Linq;
using ChangeLog.Core.Models;
using SemVer;

namespace ChangeLog.Core.Repositories
{
	public class InMemoryChangeLogRepository : IChangeLogRepository
	{
		public readonly List<ChangeLogCommit> Commits = new List<ChangeLogCommit>();

		public List<ChangeLogCommit> GetAllCommits()
		{
			return Commits;
		}

		public List<Version> GetAllVersions()
		{
			return Commits.SelectMany(c => c.Tags).Distinct().ToList();
		}
	}
}