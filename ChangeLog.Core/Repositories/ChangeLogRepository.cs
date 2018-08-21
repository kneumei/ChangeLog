using System.Collections.Generic;
using ChangeLog.Core.Models;
using SemVer;

namespace ChangeLog.Core.Repositories {
	public interface IChangeLogRepository {
		List<ChangeLogCommit> GetAllCommits();
		List<Version> GetAllVersions();
	}
}