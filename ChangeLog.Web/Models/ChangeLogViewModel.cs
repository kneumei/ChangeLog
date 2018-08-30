using SemVer;

namespace ChangeLog.Web.Models
{
	public class ChangeLogViewModel
	{
		public Version Version { get; set; }
		public string Title { get; set; }
		public string Author { get; set; }
		public string Url { get; set; }
		public string PullRequestNumber { get; set; }
		public string Category { get; set; }

	}
}