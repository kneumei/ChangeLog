﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ChangeLog.Web
{
	public class Program
	{
		public static void Main(string[] args)
		{
			SetEbConfig();
			BuildWebHost(args).Run();
		}

		public static IWebHost BuildWebHost(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>()
				.Build();

		private static void SetEbConfig()
		{
			var tempConfigBuilder = new ConfigurationBuilder();

			tempConfigBuilder.AddJsonFile(
				@"C:\Program Files\Amazon\ElasticBeanstalk\config\containerconfiguration",
				optional: true,
				reloadOnChange: true
			);

			var configuration = tempConfigBuilder.Build();

			var ebEnv =
				configuration.GetSection("iis:env")
					.GetChildren()
					.Select(pair => pair.Value.Split(new[] { '=' }, 2))
					.ToDictionary(keypair => keypair[0], keypair => keypair[1]);

			foreach (var keyVal in ebEnv)
			{
				Environment.SetEnvironmentVariable(keyVal.Key, keyVal.Value);
			}
		}
	}
}
