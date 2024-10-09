using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SolarEdge.Monitoring.Demo.Extensions;
using SolarEdge.Monitoring.Demo.Models;
using SolarEdge.Monitoring.Demo.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SolarEdge.Monitoring.Demo.Controllers;

[ApiController]
[EnableCors("EnableCORS")]
[Produces("application/json")]
[Route("api/[controller]")]
public class EnergyDetailsController(
  ILogger<EnergyDetailsController> logger,
  IEnergyDetailsService energyDetailsService)
  : ControllerBase
{

  /// <summary>
  /// Get EnergyDetails for this month
  /// </summary>
  /// <response code="200">Returns EnergyDetail month report</response>
  [HttpGet(nameof(GetEnergyDetailsThisMonthAsync))]
  [ProducesResponseType<IList<EnergyDetails>>(StatusCodes.Status200OK)]
  public async Task<IActionResult> GetEnergyDetailsThisMonthAsync()
  {
    logger.LogDebug(nameof(GetEnergyDetailsThisMonthAsync));
    IList<EnergyDetails> energyDetails = await energyDetailsService.GetEnergyDetailsThisMonthAsync().ConfigureAwait(false);
    return Ok(energyDetails);
  }

  /// <summary>
  /// Get EnergyDetails for this week
  /// </summary>
  /// <response code="200">Returns EnergyDetail week report</response>
  [HttpGet(nameof(GetEnergyDetailsThisWeekAsync))]
  [ProducesResponseType<IList<EnergyDetails>>(StatusCodes.Status200OK)]
  public async Task<IActionResult> GetEnergyDetailsThisWeekAsync()
  {
    logger.LogDebug(nameof(GetEnergyDetailsThisWeekAsync));
    IList<EnergyDetails> energyDetails = await energyDetailsService.GetEnergyDetailsThisWeekAsync().ConfigureAwait(false);
    return Ok(energyDetails);
  }

  /// <summary>
  /// Get EnergyDetails for this year
  /// </summary>
  /// <response code="200">Returns EnergyDetail year report</response>
  [HttpGet(nameof(GetEnergyDetailsThisYearAsync))]
  [ProducesResponseType<IList<EnergyDetails>>(StatusCodes.Status200OK)]
  public async Task<IActionResult> GetEnergyDetailsThisYearAsync()
  {
    logger.LogDebug(nameof(GetEnergyDetailsThisYearAsync));
    IList<EnergyDetails> energyDetails = await energyDetailsService.GetEnergyDetailsThisYearAsync().ConfigureAwait(false);
    return Ok(energyDetails);
  }

  /// <summary>
  /// Get EnergyDetails for today
  /// </summary>
  /// <response code="200">Returns EnergyDetail today report</response>
  [HttpGet(nameof(GetEnergyDetailsTodayAsync))]
  [ProducesResponseType<IList<EnergyDetails>>(StatusCodes.Status200OK)]
  public async Task<IActionResult> GetEnergyDetailsTodayAsync()
  {
    logger.LogDebug(nameof(GetEnergyDetailsTodayAsync));
    IList<EnergyDetails> energyDetailsToday = await energyDetailsService.GetEnergyDetailsTodayAsync().ConfigureAwait(false);
    return Ok(energyDetailsToday);
  }

  /// <summary>
  /// Update EnergyDetails for date range
  /// </summary>
  /// <response code="200">Returns OK result</response>
  [HttpPut(nameof(UpdateEnergyDetailsAsync))]
  [ProducesResponseType<int>(StatusCodes.Status200OK)]
  public async Task<IActionResult> UpdateEnergyDetailsAsync(DateTime dateStart, DateTime dateEnd)
  {
    logger.LogDebug(nameof(UpdateEnergyDetailsAsync));
    IEnumerable<DateTime> days = dateStart.EachDay(dateEnd);
    foreach (DateTime day in days)
    {
      await energyDetailsService.UpdateEnergyDetailsAsync(day).ConfigureAwait(false);
    }
    return Ok();
  }

  /// <summary>
  /// Update EnergyDetails for this month
  /// </summary>
  /// <response code="200">Returns OK result</response>
  [HttpPut(nameof(UpdateEnergyDetailsThisMonthAsync))]
  [ProducesResponseType<int>(StatusCodes.Status200OK)]
  public async Task<IActionResult> UpdateEnergyDetailsThisMonthAsync()
  {
    logger.LogDebug(nameof(UpdateEnergyDetailsThisMonthAsync));
    await energyDetailsService.UpdateEnergyDetailsThisMonthAsync().ConfigureAwait(false);
    return Ok();
  }

  /// <summary>
  /// Update EnergyDetails for this week
  /// </summary>
  /// <response code="200">Returns OK result</response>
  [HttpPut(nameof(UpdateEnergyDetailsThisWeekAsync))]
  [ProducesResponseType<int>(StatusCodes.Status200OK)]
  public async Task<IActionResult> UpdateEnergyDetailsThisWeekAsync()
  {
    logger.LogDebug(nameof(UpdateEnergyDetailsThisWeekAsync));
    await energyDetailsService.UpdateEnergyDetailsThisWeekAsync().ConfigureAwait(false);
    return Ok();
  }

  /// <summary>
  /// Update EnergyDetails for this year
  /// </summary>
  /// <response code="200">Returns OK result</response>
  [HttpPut(nameof(UpdateEnergyDetailsThisYearAsync))]
  [ProducesResponseType<int>(StatusCodes.Status200OK)]
  public async Task<IActionResult> UpdateEnergyDetailsThisYearAsync()
  {
    logger.LogDebug(nameof(UpdateEnergyDetailsThisYearAsync));
    await energyDetailsService.UpdateEnergyDetailsThisYearAsync().ConfigureAwait(false);
    return Ok();
  }

  /// <summary>
  /// Update EnergyDetails for today
  /// </summary>
  /// <response code="200">Returns OK result</response>
  [HttpPut(nameof(UpdateEnergyDetailsTodayAsync))]
  [ProducesResponseType<int>(StatusCodes.Status200OK)]
  public async Task<IActionResult> UpdateEnergyDetailsTodayAsync()
  {
    logger.LogDebug(nameof(UpdateEnergyDetailsTodayAsync));
    await energyDetailsService.UpdateEnergyDetailsTodayAsync().ConfigureAwait(false);
    return Ok();
  }
}
