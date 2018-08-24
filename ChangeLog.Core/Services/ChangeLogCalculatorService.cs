using System;
using System.Collections.Generic;
using System.Linq;
using ChangeLog.Core.Models;
using ChangeLog.Core.Repositories;
using SemVer;

namespace ChangeLog.Core.Services
{

	public interface IChangeLogCalculatorService
	{
		List<ChangeLogCommit> CalculateChangeLog(string beginningVersion, string endingVersion);
	}

	public class ChangeLogCalculatorService : IChangeLogCalculatorService
	{

		public ChangeLogCalculatorService(IChangeLogRepository repository)
		{
			_repository = repository;
		}

		private readonly IChangeLogRepository _repository;

		public List<ChangeLogCommit> CalculateChangeLog(string beginningVersionString, string endingVersionString)
		{
			var beginningVersion = new SemVer.Version(beginningVersionString);
			var endingVersion = new SemVer.Version(endingVersionString);

			if (beginningVersion >= endingVersion)
			{
				throw new ArgumentException($"{nameof(beginningVersion)} ({beginningVersion}) greater than {nameof(endingVersion)} ({endingVersion})");
			}

			return _repository.GetAllCommits()
				.Where(c => c.Versions.All(t => t > beginningVersion))
				.Where(c => c.Versions.Any(t => t <= endingVersion))
				.ToList();
		}
	}
}