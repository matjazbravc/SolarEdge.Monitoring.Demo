using SolarEdge.Monitoring.Demo.Models;
using SolarEdge.Monitoring.Demo.Services.Repositories.Base;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace SolarEdge.Monitoring.Demo.Services.Repositories;

public interface IEnergyDetailsRepository : IBaseRepository<EnergyDetails>
{
  Task<EnergyDetails> GetAsync(DateTime date, CancellationToken cancellationToken = default);
}
