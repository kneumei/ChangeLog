using System;
using System.Runtime.Serialization;

namespace ChangeLog.Core.Models
{

	[DataContract(Name = "labels")]
	public class PullRequestLabel
	{
		[DataMember(Name = "name")]
		public string Name { get; set; }
	}
}