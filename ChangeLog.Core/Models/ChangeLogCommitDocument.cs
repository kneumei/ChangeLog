namespace ChangeLog.Core.Models
{
	public class ChangeLogCommitDocument
	{

		public ChangeLogCommitDocument(ChangeLogCommit commit)
		{
			ChangeLogCommit = commit;
			id = commit.PullRequest.Number.ToString();
		}

		public ChangeLogCommit ChangeLogCommit { get; }
		public string id { get; }
	}

}