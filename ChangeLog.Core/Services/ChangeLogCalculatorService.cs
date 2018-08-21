using System;
using System.Collections.Generic;
using System.Linq;
using ChangeLog.Core.Models;
using ChangeLog.Core.Repositories;
using SemVer;

namespace ChangeLog.Core.Services 
{
	public class ChangeLogCalculatorService 
	{

		public ChangeLogCalculatorService(IChangeLogRepository repository){
			_repository = repository;
		}

		private readonly IChangeLogRepository _repository;

		public List<ChangeLogCommit> CalculateChangeLog(SemVer.Version beginningVersion, SemVer.Version endingVersion){
			if(beginningVersion >= endingVersion){
				throw new ArgumentException($"{nameof(beginningVersion)} ({beginningVersion}) greater than {nameof(endingVersion)} ({endingVersion})");
			}

			return _repository.GetAllCommits()
				.Where(c => c.Tags.All(t => t > beginningVersion))
				.Where(c => c.Tags.Any(t => t <= endingVersion))
				.ToList();
		}
	}
}