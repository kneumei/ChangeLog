using Xunit;
using ChangeLog.Core.Services;
using ChangeLog.Core.Repositories;
using ChangeLog.Core.Models;
using SemVer;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChangeLog.Core.Tests.Services
{
	public class ChangeLogCalculatorServiceTests
	{

		private readonly ChangeLogCalculatorService _service;
		private readonly InMemoryChangeLogRepository _repository;
		private int _id = 0;
		private int _pr = 1000;

		public ChangeLogCalculatorServiceTests()
		{
			_repository = new InMemoryChangeLogRepository();
			_service = new ChangeLogCalculatorService(_repository);
			_id = 0;
			_pr = 1000;
		}

		[Fact]
		public async Task CalculateChangeLog_DoesNotIncludeBeginningTag()
		{
			var versions = new string[][]{
				new []{"1.0.0", "1.0.1", "1.1.0", "1.1.1"}
			};
			_repository.Commits.AddRange(versions.Select(GenerateCommit).ToList());

			var changeLog = await _service.CalculateChangeLogAsync("1.0.0", "1.0.1");

			Assert.Empty(changeLog);
		}

		[Fact]
		public async Task CalculateChangeLog_IncludesNextPointRelease()
		{
			var versions = new string[][]{
				new []{"1.0.0", "1.0.1", "1.1.0", "1.1.1"},
				new []{"1.0.1", "1.1.0", "1.1.1"},
			};
			_repository.Commits.AddRange(versions.Select(GenerateCommit).ToList());

			var changeLog = await _service.CalculateChangeLogAsync("1.0.0", "1.0.1");

			Assert.Equal(1001, changeLog.Single().PullRequest.Number);
		}

		[Fact]
		public async Task CalculateChangeLog_DoesNotIncludeNextMajor()
		{
			var versions = new string[][]{
				new []{"1.0.0", "1.0.1", "1.1.0", "1.1.1"},
				new []{"1.0.1", "1.1.0", "1.1.1"},
				new []{"1.1.0", "1.1.1"},
			};
			_repository.Commits.AddRange(versions.Select(GenerateCommit).ToList());

			var changeLogs = await _service.CalculateChangeLogAsync("1.0.0", "1.0.1");
			var changeLog = changeLogs.Single();

			Assert.Equal(1001, changeLog.PullRequest.Number);
		}

		[Fact]
		public async Task CalculateChangeLog_IncludesIntermediate()
		{
			var versions = new string[][]{
				new []{"1.0.0", "1.0.1", "1.1.0", "1.1.1"},
				new []{"1.0.1", "1.1.0", "1.1.1"},
				new []{"1.1.0", "1.1.1"},
				new []{"1.1.1"},
			};
			_repository.Commits.AddRange(versions.Select(GenerateCommit).ToList());

			var changeLogs = await _service.CalculateChangeLogAsync("1.0.0", "1.1.1");
			var changeLog = changeLogs
				.OrderBy(c => c.PullRequest.Number)
				.ToList();

			Assert.Equal(3, changeLog.Count);
			Assert.Equal(1001, changeLog[0].PullRequest.Number);
			Assert.Equal(1002, changeLog[1].PullRequest.Number);
			Assert.Equal(1003, changeLog[2].PullRequest.Number);
		}

		private ChangeLogCommit GenerateCommit(IEnumerable<string> versions)
		{
			var number = _pr++;
			var pr = new PullRequest()
			{
				Id = _id++,
				Number = number,
				Title = $"#{number}"
			};
			return new ChangeLogCommit()
			{
				PullRequest = pr,
				Versions = versions.Select(v => new SemVer.Version(v)).ToList()
			};
		}
	}

	class InMemoryChangeLogRepository : IChangeLogRepository
	{

		public List<ChangeLogCommit> Commits = new List<ChangeLogCommit>();

		public async Task<List<ChangeLogCommit>> GetAllCommitsAsync()
		{
			return await Task.FromResult(Commits);
		}

		public async Task<List<Version>> GetAllVersionsAsync()
		{
			return await Task.FromResult(Commits.SelectMany(c => c.Versions).Distinct().ToList());
		}

		public Task PersistAsync(List<ChangeLogCommit> commits)
		{
			throw new System.NotImplementedException();
		}
	}
}