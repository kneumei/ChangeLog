using System.Collections.Generic;
using ChangeLog.Core.Models;
using SemVer;
using System.Threading.Tasks;

namespace ChangeLog.Core.Repositories
{
	public interface IChangeLogRepository
	{
		List<ChangeLogCommit> GetAllCommits();
		List<Version> GetAllVersions();
		Task PersistAsync(List<ChangeLogCommit> commits);
	}
}