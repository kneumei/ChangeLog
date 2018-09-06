using Microsoft.AspNetCore.Mvc;
using ChangeLog.Core.Services;
using ChangeLog.Core.Repositories;
using ChangeLog.Core.Models;
using System.Linq;
using SemVer;
using System.Collections.Generic;
using ChangeLog.Web.Models;
using System.Threading.Tasks;

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

		public async Task<IActionResult> Index(string beginVersion, string endVersion)
		{
			var versionResults = await _repository.GetAllVersionsAsync();
			var versions = versionResults.OrderBy(v => v).ToList();
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
				var changeLogResults = await _calculatorService.CalculateChangeLogAsync(beginSemVer.ToString(), endSemVer.ToString());
				commits = changeLogResults
					.OrderByDescending(c => c.Versions.Min())
					.Select(c => new ChangeLogViewModel()
					{
						Version = c.Versions.Min(),
						Title = c.PullRequest.Title,
						Author = c.PullRequest.User.Login,
						PullRequestNumber = c.PullRequest.Number.ToString(),
						Url = c.PullRequest.Url,
						Category = c.PullRequest.Labels.FirstOrDefault()?.Name ?? "Other"
					})
					.ToList();

			}

			var allVersions = versions
				.OrderByDescending(v => v)
				.Select(v => v.ToString())
				.ToList();

			var model = new HomeViewModel()
			{
				Commits = commits,
				AllVersions = allVersions,
				BeginVersion = beginSemVer,
				EndVersion = endSemVer
			};

			return View(model);
		}
	}

}