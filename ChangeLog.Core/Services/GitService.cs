using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ChangeLog.Core.Models;
using ChangeLog.Core.Settings;

namespace ChangeLog.Core.Services
{

	interface IGitService
	{
		List<ChangeLogCommit> GetChangeLogCommits(List<PullRequest> pullRequests);
	}

	public class GitService : IGitService
	{
		private readonly GitSettings _settings;
		public GitService(GitSettings settings)
		{
			_settings = settings;
		}

		public List<ChangeLogCommit> GetChangeLogCommits(List<PullRequest> pullRequests)
		{
			var commits = new List<ChangeLogCommit>();

			foreach (var pr in pullRequests)
			{
				var startInfo = CreateProcessStartInfo(pr);

				using (var process = Process.Start(startInfo))
				{
					var output = process.StandardOutput.ReadToEnd();
					var error = process.StandardError.ReadToEnd();

					process.WaitForExit();

					var tags = output.Split(Environment.NewLine.ToCharArray())
						.Where(t => !String.IsNullOrWhiteSpace(t));

					var commit = new ChangeLogCommit(pr, new List<string>(tags));

					commits.Add(commit);
				};
			}
			return commits;
		}

		private ProcessStartInfo CreateProcessStartInfo(PullRequest pr)
		{
			var startInfo = new ProcessStartInfo();
			startInfo.FileName = "git.exe";
			startInfo.WorkingDirectory = _settings.RepositoryPath;//@"C:\Users\Kyle\Documents\Github\ChangeLogTest";
			startInfo.Arguments = $"tag --contains {pr.MergeCommitSha}";
			//startInfo.Arguments = "status";
			startInfo.RedirectStandardOutput = true;
			startInfo.RedirectStandardError = true;
			startInfo.UseShellExecute = false;
			return startInfo;
		}
	}
}