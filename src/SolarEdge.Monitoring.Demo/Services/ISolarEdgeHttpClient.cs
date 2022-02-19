using SolarEdge.Monitoring.Demo.Models.Dto;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace SolarEdge.Monitoring.Demo.Services;

public interface ISolarEdgeHttpClient
{
  Task<OverviewDto> GetOverviewInfoAsync(string siteId, CancellationToken cancellationToken = default);

  Task<EnergyDetailsDto> GetEnergyDetailsAsync(string siteId, DateTime start, DateTime end, CancellationToken cancellationToken = default);

  Task<PowerDetailsDto> GetPowerDetailsAsync(string siteId, DateTime start, DateTime end, CancellationToken cancellationToken = default);
}