using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SolarEdge.Monitoring.Demo.Models;
using SolarEdge.Monitoring.Demo.Services;
using System.Threading.Tasks;

namespace SolarEdge.Monitoring.Demo.Controllers;

[ApiController]
[EnableCors("EnableCORS")]
[Produces("application/json")]
[Route("api/[controller]")]
public class OverviewController(
  ILogger<OverviewController> logger,
  IOverviewService overviewService)
  : ControllerBase
{

  /// <summary>
  /// Get Overview information
  /// </summary>
  /// <response code="200">Returns Overview information</response>
  [HttpGet(nameof(GetOverviewAsync))]
  [ProducesResponseType<Overview>(StatusCodes.Status200OK)]
  public async Task<IActionResult> GetOverviewAsync()
  {
    Overview overview = await overviewService.GetOverviewAsync().ConfigureAwait(false);
    return Ok(overview);
  }

  /// <summary>
  /// Update Overview information
  /// </summary>
  /// <response code="200">Returns OK result</response>
  [HttpPut(nameof(UpdateOverviewAsync))]
  [ProducesResponseType<int>(StatusCodes.Status200OK)]
  public async Task<IActionResult> UpdateOverviewAsync()
  {
    logger.LogDebug(nameof(UpdateOverviewAsync));
    await overviewService.UpdateOverviewAsync().ConfigureAwait(false);
    return Ok();
  }
}
