using Microsoft.AspNetCore.Mvc;
using ChangeLog.Core.Services;
using ChangeLog.Core.Repositories;
using ChangeLog.Core.Models;
using System.Linq;
using SemVer;
using System.Collections.Generic;
using ChangeLog.Web.Models;

namespace ChangeLog.Web.Controllers
{
	public class HomeController : Controller
	{
		private readonly IChangeLogCalculatorService _calculatorService;
		private readonly IChangeLogRepository _repository;

		public HomeController(IChangeLogCalculatorService calculatorService, IChangeLogRepository repository)
		{
			_calculatorService = calculatorService;
			_repository = repository;
		}

		public IActionResult Index(string beginVersion, string endVersion)
		{
			var versions = _repository.GetAllVersions().OrderBy(v => v).ToList();
			Version beginSemVer = null;
			Version endSemVer = null;
			var commits = new List<ChangeLogViewModel>();

			if (!string.IsNullOrEmpty(beginVersion))
			{
				beginSemVer = new Version(beginVersion);
			}

			if (!string.IsNullOrEmpty(endVersion))
			{
				endSemVer = new Version(endVersion);
			}

			if (beginSemVer != null && endSemVer != null)
			{
				commits = _calculatorService
					.CalculateChangeLog(beginSemVer.ToString(), endSemVer.ToString())
					.OrderByDescending(c => c.Tags.Min())
					.Select(c => new ChangeLogViewModel()
					{
						Version = c.Tags.Min(),
						Title = c.PullRequest.Title,
						Author = c.PullRequest.User.Login,
						PullRequestNumber = c.PullRequest.Number.ToString(),
						Url = c.PullRequest.Url
					})
					.ToList();

			}

			var model = new HomeViewModel()
			{
				Commits = commits,
				AllVersions = versions,
				BeginVersion = beginSemVer,
				EndVersion = endSemVer
			};

			return View(model);
		}
	}

}