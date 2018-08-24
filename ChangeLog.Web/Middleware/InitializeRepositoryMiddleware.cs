using Microsoft.AspNetCore.Http;
using ChangeLog.Core.Repositories;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.Threading;

public class InitializeRepositoryMiddleware
{
	private readonly RequestDelegate _next;
	private readonly IChangeLogRepository _chageLogRepository;
	private Task _initializationTask;

	public InitializeRepositoryMiddleware(
		RequestDelegate next,
		IApplicationLifetime lifetime,
		IChangeLogRepository changeLogRepository)
	{
		_next = next;
		_chageLogRepository = changeLogRepository;

		var startRegistration = default(CancellationTokenRegistration);
		startRegistration = lifetime.ApplicationStarted.Register(() =>
		{
			if (_chageLogRepository is IInitializeable)
			{
				_initializationTask = InitializeAsync(lifetime.ApplicationStopping);
			}
			else
			{
				_initializationTask = Task.CompletedTask;
			}

			startRegistration.Dispose();
		});
	}

	private async Task InitializeAsync(CancellationToken cancellationToken)
	{
		var repo = _chageLogRepository as DirectAccessChangeLogRepository;
		await repo.InitializeAsync();
	}

	public async Task Invoke(HttpContext context)
	{
		await _initializationTask;
		await _next(context);
	}


}