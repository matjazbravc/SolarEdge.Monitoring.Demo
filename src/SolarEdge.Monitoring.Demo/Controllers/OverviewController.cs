using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SolarEdge.Monitoring.Demo.Services;
using System.Threading.Tasks;

namespace SolarEdge.Monitoring.Demo.Controllers
{
	[ApiController]
	[EnableCors("EnableCORS")]
	[Produces("application/json")]
	[Route("api/[controller]")]
	public class OverviewController : ControllerBase
	{
		private readonly IOverviewService _overviewService;
		private readonly ILogger<OverviewController> _logger;

		public OverviewController(ILogger<OverviewController> logger,
			IOverviewService overviewService)
		{
			_logger = logger;
			_overviewService = overviewService;
		}

		/// <summary>
		/// Get Overview information
		/// </summary>
		/// <response code="200">Returns Overview information</response>
		[HttpGet(nameof(GetOverviewAsync))]
		public async Task<IActionResult> GetOverviewAsync()
		{
			var overview = await _overviewService.GetOverviewAsync().ConfigureAwait(false);
			return Ok(overview);
		}

		/// <summary>
		/// Update Overview information
		/// </summary>
		/// <response code="200">Returns OK result</response>
		[HttpPut(nameof(UpdateOverviewAsync))]
		public async Task<IActionResult> UpdateOverviewAsync()
		{
			_logger.LogDebug(nameof(UpdateOverviewAsync));
			await _overviewService.UpdateOverviewAsync().ConfigureAwait(false);
			return Ok();
		}
	}
}
