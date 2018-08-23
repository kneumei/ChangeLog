using System;
using System.Runtime.Serialization;

namespace ChangeLog.Core.Models
{

	[DataContract(Name = "user")]
	public class PullRequestUser
	{
		[DataMember(Name = "login")]
		public string Login { get; set; }
	}
}