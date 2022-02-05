using Quartz;
using System.Threading.Tasks;

namespace SolarEdge.Monitoring.Demo.Services.Quartz
{
	[DisallowConcurrentExecution]
	public class SolarEdgeGetEnergyDetailsJob : IJob
	{
		private readonly IEnergyDetailsService _energyDetailsService;

		public SolarEdgeGetEnergyDetailsJob(IEnergyDetailsService energyDetailsService)
		{
			_energyDetailsService = energyDetailsService;
		}

		public async Task Execute(IJobExecutionContext context)
		{
			await _energyDetailsService.UpdateEnergyDetailsTodayAsync(context.CancellationToken).ConfigureAwait(false);
		}
	}
}