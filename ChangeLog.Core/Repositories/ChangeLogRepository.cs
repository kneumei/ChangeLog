using System.Collections.Generic;
using ChangeLog.Core.Models;
using SemVer;
using System.Threading.Tasks;

namespace ChangeLog.Core.Repositories
{
	public interface IChangeLogRepository
	{
		Task<List<ChangeLogCommit>> GetAllCommitsAsync();
		Task<List<Version>> GetAllVersionsAsync();
		Task PersistAsync(List<ChangeLogCommit> commits);
	}
}