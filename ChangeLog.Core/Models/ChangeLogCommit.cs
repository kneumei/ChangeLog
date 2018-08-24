using System.Collections.Generic;
using System.Linq;
using SemVer;
using Newtonsoft.Json;
using ChangeLog.Core.Utilities;

namespace ChangeLog.Core.Models
{
	public class ChangeLogCommit
	{
		public ChangeLogCommit(PullRequest pullRequest, List<string> gitTags)
		{
			PullRequest = pullRequest;
			Versions = gitTags.Select(t => new Version(t)).ToList();
		}

		[JsonProperty(ItemConverterType = typeof(VersionJsonConvertor))]
		public List<Version> Versions { get; }

		public PullRequest PullRequest { get; }
	}
}