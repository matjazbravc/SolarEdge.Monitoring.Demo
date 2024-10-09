using SolarEdge.Monitoring.Demo.Models;
using SolarEdge.Monitoring.Demo.Services.Repositories.Base;
using System.Threading.Tasks;
using System.Threading;

namespace SolarEdge.Monitoring.Demo.Services.Repositories;

public interface IOverviewRepository : IBaseRepository<Overview>
{
  Task<Overview> GetFirstAsync(bool disableTracking = true, CancellationToken cancellationToken = default);
}
