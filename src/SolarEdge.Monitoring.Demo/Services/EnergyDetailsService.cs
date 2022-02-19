using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SolarEdge.Monitoring.Demo.Extensions;
using SolarEdge.Monitoring.Demo.Models;
using SolarEdge.Monitoring.Demo.Models.Dto;
using SolarEdge.Monitoring.Demo.Services.Configuration;
using SolarEdge.Monitoring.Demo.Services.Converters;
using SolarEdge.Monitoring.Demo.Services.Repositories;

namespace SolarEdge.Monitoring.Demo.Services;

public class EnergyDetailsService(
  ILogger<EnergyDetailsService> logger,
  IOptions<ServiceConfig> config, 
  ISolarEdgeHttpClient solarEdgeHttpClient,
  IEnergyDetailsRepository energyDetailsRepository,
  IConverter<EnergyDetailsDto, EnergyDetails> energyDetailsConverter)
  : IEnergyDetailsService
{
  private readonly ServiceConfig _config = config.Value;

  public async Task<IList<EnergyDetails>> GetEnergyDetailsThisMonthAsync(CancellationToken cancellationToken)
  {
    var startTime = DateTime.Today.StartOfThisMonth();
    var energyDetails = await energyDetailsRepository.GetAllAsync(p =>
      p.Time.Date >= startTime.Date && p.Time.Date <= DateTime.Today.Date, cancellationToken: cancellationToken);
    return [.. energyDetails.OrderBy(by => by.Time)];
  }

  public async Task<IList<EnergyDetails>> GetEnergyDetailsThisWeekAsync(CancellationToken cancellationToken)
  {
    var startTime = DateTime.Today.StartOfThisWeek();
    var energyDetails = await energyDetailsRepository.GetAllAsync(p =>
      (p.Time.Date >= startTime.Date && p.Time.Date <= DateTime.Today.Date), cancellationToken: cancellationToken);
    return [.. energyDetails.OrderBy(by => by.Time)];
  }

  public async Task<IList<EnergyDetails>> GetEnergyDetailsThisYearAsync(CancellationToken cancellationToken)
  {
    var startTime = DateTime.Today.StartOfThisYear();
    var energyDetails = await energyDetailsRepository.GetAllAsync(p =>
      (p.Time.Date >= startTime.Date && p.Time.Date <= DateTime.Today.Date), cancellationToken: cancellationToken);
    return [.. energyDetails.OrderBy(by => by.Time)];
  }

  public async Task<IList<EnergyDetails>> GetEnergyDetailsTodayAsync(CancellationToken cancellationToken = default)
  {
    var energyDetails = await energyDetailsRepository.GetAllAsync(p => p.Time.Date == DateTime.Today.Date, cancellationToken: cancellationToken);
    return energyDetails;
  }

  public async Task UpdateEnergyDetailsAsync(DateTime date, CancellationToken cancellationToken = default)
  {
    var energyDetailsResult = await solarEdgeHttpClient.GetEnergyDetailsAsync(_config.SolarEdgeSiteId, date.StartOfDay(), date.EndOfDay(), cancellationToken).ConfigureAwait(false);
    var energyDetails = energyDetailsConverter.Convert(energyDetailsResult);

    // Add/update EnergyDetails table
    var existingEnergyDetails = await energyDetailsRepository.GetAsync(date, cancellationToken).ConfigureAwait(false);
    if (existingEnergyDetails == null)
    {
      await energyDetailsRepository.AddAsync(energyDetails, cancellationToken).ConfigureAwait(false);
    }
    else
    {
      energyDetails.Id = existingEnergyDetails.Id;
      await energyDetailsRepository.UpdateAsync(energyDetails, cancellationToken: cancellationToken).ConfigureAwait(false);
    }
  }

  public async Task UpdateEnergyDetailsThisMonthAsync(CancellationToken cancellationToken = default)
  {
    logger.LogInformation($"Start {nameof(UpdateEnergyDetailsThisMonthAsync)} at {DateTime.UtcNow.ToSqlDateTime()}");
    var days = DateTime.Today.StartOfThisMonth().EachDay(DateTime.Today);
    foreach (var day in days)
    {
      await UpdateEnergyDetailsAsync(day, cancellationToken).ConfigureAwait(false);
    }
    logger.LogInformation($"End {nameof(UpdateEnergyDetailsThisMonthAsync)} at {DateTime.UtcNow.ToSqlDateTime()}");
  }

  public async Task UpdateEnergyDetailsThisWeekAsync(CancellationToken cancellationToken = default)
  {
    logger.LogInformation($"Start {nameof(UpdateEnergyDetailsThisWeekAsync)} at {DateTime.UtcNow.ToSqlDateTime()}");
    var days = DateTime.Today.StartOfThisWeek().EachDay(DateTime.Today);
    foreach (var day in days)
    {
      await UpdateEnergyDetailsAsync(day, cancellationToken).ConfigureAwait(false);
    }
    logger.LogInformation($"End {nameof(UpdateEnergyDetailsThisWeekAsync)} at {DateTime.UtcNow.ToSqlDateTime()}");
  }

  public async Task UpdateEnergyDetailsThisYearAsync(CancellationToken cancellationToken = default)
  {
    logger.LogInformation($"Start {nameof(UpdateEnergyDetailsThisYearAsync)} at {DateTime.UtcNow.ToSqlDateTime()}");
    var days = DateTime.Today.StartOfThisYear().EachDay(DateTime.Today);
    foreach (var day in days)
    {
      await UpdateEnergyDetailsAsync(day, cancellationToken).ConfigureAwait(false);
    }
    logger.LogInformation($"End {nameof(UpdateEnergyDetailsThisYearAsync)} at {DateTime.UtcNow.ToSqlDateTime()}");
  }

  public async Task UpdateEnergyDetailsTodayAsync(CancellationToken cancellationToken = default)
  {
    logger.LogInformation($"Start {nameof(UpdateEnergyDetailsTodayAsync)} at {DateTime.UtcNow.ToSqlDateTime()}");
    await UpdateEnergyDetailsAsync(DateTime.Today, cancellationToken).ConfigureAwait(false);
    logger.LogInformation($"End {nameof(UpdateEnergyDetailsTodayAsync)} at {DateTime.UtcNow.ToSqlDateTime()}");
  }
}
