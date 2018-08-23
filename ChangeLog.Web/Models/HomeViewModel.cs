using SemVer;
using System.Collections.Generic;
using ChangeLog.Core.Models;

namespace ChangeLog.Web.Models
{
	public class HomeViewModel
	{

		public List<ChangeLogViewModel> Commits { get; set; }
		public List<Version> AllVersions { get; set; }
		public Version BeginVersion { get; set; }
		public Version EndVersion { get; set; }

	}
}