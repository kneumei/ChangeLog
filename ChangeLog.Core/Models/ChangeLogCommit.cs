using System.Collections.Generic;
using System.Linq;
using SemVer;

namespace ChangeLog.Core.Models
{
	public class ChangeLogCommit
	{
		public ChangeLogCommit(PullRequest pullRequest, List<string> tags)
		{
			PullRequest = pullRequest;
			Tags = tags.Select(t => new Version(t)).ToList();
		}

		public List<Version> Tags { get; }

		public PullRequest PullRequest { get; }
	}
}