using SolarEdge.Monitoring.Demo.Models;
using System.Threading;
using System.Threading.Tasks;

namespace SolarEdge.Monitoring.Demo.Services;

public interface IOverviewService
{
  Task UpdateOverviewAsync(CancellationToken cancellationToken = default);

  Task<Overview> GetOverviewAsync(CancellationToken cancellationToken = default);
}
