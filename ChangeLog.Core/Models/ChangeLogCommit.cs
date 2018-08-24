using System.Collections.Generic;
using System.Linq;
using SemVer;
using Newtonsoft.Json;
using ChangeLog.Core.Utilities;

namespace ChangeLog.Core.Models
{
	public class ChangeLogCommit
	{

		public ChangeLogCommit()
		{
			//serializer
		}

		[JsonProperty(ItemConverterType = typeof(VersionJsonConvertor))]
		public List<Version> Versions { get; set; }

		public PullRequest PullRequest { get; set; }
	}
}