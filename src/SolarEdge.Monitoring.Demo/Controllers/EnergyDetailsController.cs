using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SolarEdge.Monitoring.Demo.Extensions;
using SolarEdge.Monitoring.Demo.Services;
using System.Threading.Tasks;
using System;

namespace SolarEdge.Monitoring.Demo.Controllers
{
	[ApiController]
	[EnableCors("EnableCORS")]
	[Produces("application/json")]
	[Route("api/[controller]")]
	public class EnergyDetailsController : ControllerBase
	{
		private readonly IEnergyDetailsService _energyDetailsService;
		private readonly ILogger<EnergyDetailsController> _logger;
		
		public EnergyDetailsController(ILogger<EnergyDetailsController> logger,
			IEnergyDetailsService energyDetailsService)
		{
			_logger = logger;
			_energyDetailsService = energyDetailsService;
		}

		/// <summary>
		/// Get EnergyDetails for this month
		/// </summary>
		/// <response code="200">Returns EnergyDetail month report</response>
		[HttpGet(nameof(GetEnergyDetailsThisMonthAsync))]
		public async Task<IActionResult> GetEnergyDetailsThisMonthAsync()
		{
			_logger.LogDebug(nameof(GetEnergyDetailsThisMonthAsync));
			var energyDetails = await _energyDetailsService.GetEnergyDetailsThisMonthAsync().ConfigureAwait(false);
			return Ok(energyDetails);
		}

		/// <summary>
		/// Get EnergyDetails for this week
		/// </summary>
		/// <response code="200">Returns EnergyDetail week report</response>
		[HttpGet(nameof(GetEnergyDetailsThisWeekAsync))]
		public async Task<IActionResult> GetEnergyDetailsThisWeekAsync()
		{
			_logger.LogDebug(nameof(GetEnergyDetailsThisWeekAsync));
			var energyDetails = await _energyDetailsService.GetEnergyDetailsThisWeekAsync().ConfigureAwait(false);
			return Ok(energyDetails);
		}

		/// <summary>
		/// Get EnergyDetails for this year
		/// </summary>
		/// <response code="200">Returns EnergyDetail year report</response>
		[HttpGet(nameof(GetEnergyDetailsThisYearAsync))]
		public async Task<IActionResult> GetEnergyDetailsThisYearAsync()
		{
			_logger.LogDebug(nameof(GetEnergyDetailsThisYearAsync));
			var energyDetails = await _energyDetailsService.GetEnergyDetailsThisYearAsync().ConfigureAwait(false);
			return Ok(energyDetails);
		}

		/// <summary>
		/// Get EnergyDetails for today
		/// </summary>
		/// <response code="200">Returns EnergyDetail today report</response>
		[HttpGet(nameof(GetEnergyDetailsTodayAsync))]
		public async Task<IActionResult> GetEnergyDetailsTodayAsync()
		{
			_logger.LogDebug(nameof(GetEnergyDetailsTodayAsync));
			var energyDetailsToday = await _energyDetailsService.GetEnergyDetailsTodayAsync().ConfigureAwait(false);
			return Ok(energyDetailsToday);
		}

		/// <summary>
		/// Update EnergyDetails for date range
		/// </summary>
		/// <response code="200">Returns OK result</response>
		[HttpPut(nameof(UpdateEnergyDetailsAsync))]
		public async Task<IActionResult> UpdateEnergyDetailsAsync(DateTime dateStart, DateTime dateEnd)
		{
			_logger.LogDebug(nameof(UpdateEnergyDetailsAsync));
			var days = dateStart.EachDay(dateEnd);
			foreach (var day in days)
			{
				await _energyDetailsService.UpdateEnergyDetailsAsync(day).ConfigureAwait(false);
			}
			return Ok();
		}

		/// <summary>
		/// Update EnergyDetails for this month
		/// </summary>
		/// <response code="200">Returns OK result</response>
		[HttpPut(nameof(UpdateEnergyDetailsThisMonthAsync))]
		public async Task<IActionResult> UpdateEnergyDetailsThisMonthAsync()
		{
			_logger.LogDebug(nameof(UpdateEnergyDetailsThisMonthAsync));
			await _energyDetailsService.UpdateEnergyDetailsThisMonthAsync().ConfigureAwait(false);
			return Ok();
		}

		/// <summary>
		/// Update EnergyDetails for this week
		/// </summary>
		/// <response code="200">Returns OK result</response>
		[HttpPut(nameof(UpdateEnergyDetailsThisWeekAsync))]
		public async Task<IActionResult> UpdateEnergyDetailsThisWeekAsync()
		{
			_logger.LogDebug(nameof(UpdateEnergyDetailsThisWeekAsync));
			await _energyDetailsService.UpdateEnergyDetailsThisWeekAsync().ConfigureAwait(false);
			return Ok();
		}

		/// <summary>
		/// Update EnergyDetails for this year
		/// </summary>
		/// <response code="200">Returns OK result</response>
		[HttpPut(nameof(UpdateEnergyDetailsThisYearAsync))]
		public async Task<IActionResult> UpdateEnergyDetailsThisYearAsync()
		{
			_logger.LogDebug(nameof(UpdateEnergyDetailsThisYearAsync));
			await _energyDetailsService.UpdateEnergyDetailsThisYearAsync().ConfigureAwait(false);
			return Ok();
		}

		/// <summary>
		/// Update EnergyDetails for today
		/// </summary>
		/// <response code="200">Returns OK result</response>
		[HttpPut(nameof(UpdateEnergyDetailsTodayAsync))]
		public async Task<IActionResult> UpdateEnergyDetailsTodayAsync()
		{
			_logger.LogDebug(nameof(UpdateEnergyDetailsTodayAsync));
			await _energyDetailsService.UpdateEnergyDetailsTodayAsync().ConfigureAwait(false);
			return Ok();
		}
	}
}
