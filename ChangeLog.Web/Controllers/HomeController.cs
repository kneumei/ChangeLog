using Microsoft.AspNetCore.Mvc;
using ChangeLog.Core.Services;

namespace ChangeLog.Web.Controllers
{
	public class HomeController : Controller
	{
		private readonly IChangeLogCalculatorService _calculatorService;
		public HomeController(IChangeLogCalculatorService calculatorService)
		{
			_calculatorService = calculatorService;
		}

		public IActionResult Index(string beginVersion, string endVersion)
		{
			return View();
		}
	}

}