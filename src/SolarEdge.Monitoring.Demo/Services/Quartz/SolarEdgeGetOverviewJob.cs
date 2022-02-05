using Quartz;
using System.Threading.Tasks;

namespace SolarEdge.Monitoring.Demo.Services.Quartz
{
	[DisallowConcurrentExecution]
	public class SolarEdgeGetOverviewJob : IJob
	{
		private readonly IOverviewService _overviewService;

		public SolarEdgeGetOverviewJob(IOverviewService overviewService)
		{
			_overviewService = overviewService;
		}

		public async Task Execute(IJobExecutionContext context)
		{
			await _overviewService.UpdateOverviewAsync(context.CancellationToken).ConfigureAwait(false);
		}
	}
}