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

namespace SolarEdge.Monitoring.Demo.Services
{
	public class EnergyDetailsService : IEnergyDetailsService
	{
		private readonly ServiceConfig _config;
		private readonly IConverter<EnergyDetailsDto, EnergyDetails> _energyDetailsConverter;
		private readonly IEnergyDetailsRepository _energyDetailsRepository;
		private readonly ILogger<EnergyDetailsService> _logger;
		private readonly ISolarEdgeHttpClient _solarEdgeHttpClient;

		public EnergyDetailsService(ILogger<EnergyDetailsService> logger,
			IOptions<ServiceConfig> config, ISolarEdgeHttpClient solarEdgeHttpClient,
			IEnergyDetailsRepository energyDetailsRepository,
			IConverter<EnergyDetailsDto, EnergyDetails> energyDetailsConverter)
		{
			_logger = logger;
			_config = config.Value;
			_solarEdgeHttpClient = solarEdgeHttpClient;
			_energyDetailsRepository = energyDetailsRepository;
			_energyDetailsConverter = energyDetailsConverter;
		}

		public async Task<IList<EnergyDetails>> GetEnergyDetailsThisMonthAsync(CancellationToken cancellationToken)
		{
			var startTime = DateTime.Today.StartOfThisMonth();
			var energyDetails = await _energyDetailsRepository.GetAllAsync(p =>
				(p.Time.Date >= startTime.Date && p.Time.Date <= DateTime.Today.Date), cancellationToken);
			return energyDetails.OrderBy(by => by.Time).ToList();
		}

		public async Task<IList<EnergyDetails>> GetEnergyDetailsThisWeekAsync(CancellationToken cancellationToken)
		{
			var startTime = DateTime.Today.StartOfThisWeek();
			var energyDetails = await _energyDetailsRepository.GetAllAsync(p =>
				(p.Time.Date >= startTime.Date && p.Time.Date <= DateTime.Today.Date), cancellationToken);
			return energyDetails.OrderBy(by => by.Time).ToList();
		}

		public async Task<IList<EnergyDetails>> GetEnergyDetailsThisYearAsync(CancellationToken cancellationToken)
		{
			var startTime = DateTime.Today.StartOfThisYear();
			var energyDetails = await _energyDetailsRepository.GetAllAsync(p =>
				(p.Time.Date >= startTime.Date && p.Time.Date <= DateTime.Today.Date), cancellationToken);
			return energyDetails.OrderBy(by => by.Time).ToList();
		}

		public async Task<IList<EnergyDetails>> GetEnergyDetailsTodayAsync(CancellationToken cancellationToken = default)
		{
			var energyDetails = await _energyDetailsRepository.GetAllAsync(p => p.Time.Date == DateTime.Today.Date, cancellationToken);
			return energyDetails;
		}

		public async Task UpdateEnergyDetailsAsync(DateTime date, CancellationToken cancellationToken = default)
		{
			var energyDetailsResult = await _solarEdgeHttpClient.GetEnergyDetailsAsync(_config.SolarEdgeSiteId, date.StartOfDay(), date.EndOfDay(), cancellationToken).ConfigureAwait(false);
			var energyDetails = _energyDetailsConverter.Convert(energyDetailsResult);

			// Add/update EnergyDetails table
			var existingEnergyDetails = await _energyDetailsRepository.GetAsync(date, cancellationToken).ConfigureAwait(false);
			if (existingEnergyDetails == null)
			{
				await _energyDetailsRepository.AddAsync(energyDetails, cancellationToken).ConfigureAwait(false);
			}
			else
			{
				energyDetails.Id = existingEnergyDetails.Id;
				await _energyDetailsRepository.UpdateAsync(energyDetails, cancellationToken).ConfigureAwait(false);
			}
		}

		public async Task UpdateEnergyDetailsThisMonthAsync(CancellationToken cancellationToken = default)
		{
			_logger.LogInformation($"Start {nameof(UpdateEnergyDetailsThisMonthAsync)} at {DateTime.UtcNow.ToSqlDateTime()}");
			var days = DateTime.Today.StartOfThisMonth().EachDay(DateTime.Today); 
			foreach (var day in days)
			{
				await UpdateEnergyDetailsAsync(day, cancellationToken).ConfigureAwait(false);
			}
			_logger.LogInformation($"End {nameof(UpdateEnergyDetailsThisMonthAsync)} at {DateTime.UtcNow.ToSqlDateTime()}");
		}

		public async Task UpdateEnergyDetailsThisWeekAsync(CancellationToken cancellationToken = default)
		{
			_logger.LogInformation($"Start {nameof(UpdateEnergyDetailsThisWeekAsync)} at {DateTime.UtcNow.ToSqlDateTime()}");
			var days = DateTime.Today.StartOfThisWeek().EachDay(DateTime.Today);
			foreach (var day in days)
			{
				await UpdateEnergyDetailsAsync(day, cancellationToken).ConfigureAwait(false);
			}
			_logger.LogInformation($"End {nameof(UpdateEnergyDetailsThisWeekAsync)} at {DateTime.UtcNow.ToSqlDateTime()}");
		}

		public async Task UpdateEnergyDetailsThisYearAsync(CancellationToken cancellationToken = default)
		{
			_logger.LogInformation($"Start {nameof(UpdateEnergyDetailsThisYearAsync)} at {DateTime.UtcNow.ToSqlDateTime()}");
			var days = DateTime.Today.StartOfThisYear().EachDay(DateTime.Today);
			foreach (var day in days)
			{
				await UpdateEnergyDetailsAsync(day, cancellationToken).ConfigureAwait(false);
			}
			_logger.LogInformation($"End {nameof(UpdateEnergyDetailsThisYearAsync)} at {DateTime.UtcNow.ToSqlDateTime()}");
		}

		public async Task UpdateEnergyDetailsTodayAsync(CancellationToken cancellationToken = default)
		{
			_logger.LogInformation($"Start {nameof(UpdateEnergyDetailsTodayAsync)} at {DateTime.UtcNow.ToSqlDateTime()}");
			await UpdateEnergyDetailsAsync(DateTime.Today, cancellationToken).ConfigureAwait(false);
			_logger.LogInformation($"End {nameof(UpdateEnergyDetailsTodayAsync)} at {DateTime.UtcNow.ToSqlDateTime()}");
		}
	}
}
