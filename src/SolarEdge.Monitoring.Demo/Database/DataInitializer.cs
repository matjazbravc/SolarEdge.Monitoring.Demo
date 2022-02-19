using System.Linq;
using System.Threading.Tasks;
using SolarEdge.Monitoring.Demo.Services;

namespace SolarEdge.Monitoring.Demo.Database;

public class DataInitializer(
  DataContext context, 
  IEnergyDetailsService energyDetailsService, 
  IOverviewService overviewService)
  : IDataInitializer
{
  public async Task InitializeAsync()
  {
    await context.Database.EnsureCreatedAsync();

    if (context.EnergyDetails.Any() && context.Overview.Any())
    {
      return;
    }

    await overviewService.UpdateOverviewAsync();
    await energyDetailsService.UpdateEnergyDetailsThisWeekAsync();
  }
}
