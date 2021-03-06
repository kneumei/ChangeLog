using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;

namespace ChangeLog.Core.Models
{

	[DataContract(Name = "pulls")]
	public class PullRequest
	{

		[DataMember(Name = "id")]
		public int Id { get; set; }

		[DataMember(Name = "number")]
		public int Number { get; set; }

		[DataMember(Name = "title")]
		public string Title { get; set; }

		[DataMember(Name = "merged_at")]
		private string MergedAtRaw { get; set; }

		[DataMember(Name = "merge_commit_sha")]
		public string MergeCommitSha { get; set; }

		[DataMember(Name = "labels")]
		public List<PullRequestLabel> Labels { get; set; }

		[DataMember(Name = "user")]
		public PullRequestUser User { get; set; }

		[DataMember(Name = "html_url")]
		public string Url { get; set; }

		[DataMember(Name = "body")]
		public string Body { get; set; }

		[IgnoreDataMember]
		public DateTime MergedAt
		{
			get
			{
				return DateTime.ParseExact(MergedAtRaw, "yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
			}
		}


	}
}