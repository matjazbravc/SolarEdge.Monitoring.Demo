using System.Linq;
using System.Threading.Tasks;
using SolarEdge.Monitoring.Demo.Services;

namespace SolarEdge.Monitoring.Demo.Database
{
	public class DataInitializer : IDataInitializer
	{
		private readonly DataContext _context;
		private readonly IEnergyDetailsService _energyDetailsService;
		private readonly IOverviewService _overviewService;

		public DataInitializer(DataContext context, IEnergyDetailsService energyDetailsService, IOverviewService overviewService)
		{
			_context = context;
			_energyDetailsService = energyDetailsService;
			_overviewService = overviewService;
		}

		public async Task Initialize()
		{
			await _context.Database.EnsureCreatedAsync();

			if (_context.EnergyDetails.Any() && _context.Overview.Any())
			{
				return;
			}

			await _overviewService.UpdateOverviewAsync();
			await _energyDetailsService.UpdateEnergyDetailsThisWeekAsync();
		}
	}
}
