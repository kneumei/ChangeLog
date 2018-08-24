using Newtonsoft.Json;

namespace ChangeLog.Core.Models
{
	public class ChangeLogCommitDocument
	{

		[JsonConstructor]
		public ChangeLogCommitDocument() { }

		public ChangeLogCommitDocument(ChangeLogCommit commit)
		{
			ChangeLogCommit = commit;
			id = commit.PullRequest.Number.ToString();
		}

		public ChangeLogCommit ChangeLogCommit { get; set; }
		public string id { get; set; }
	}

}