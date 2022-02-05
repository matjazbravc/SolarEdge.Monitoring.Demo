using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SolarEdge.Monitoring.Demo.Extensions;
using SolarEdge.Monitoring.Demo.Models.Dto;
using SolarEdge.Monitoring.Demo.Models;
using SolarEdge.Monitoring.Demo.Services.Configuration;
using SolarEdge.Monitoring.Demo.Services.Converters;
using SolarEdge.Monitoring.Demo.Services.Repositories;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace SolarEdge.Monitoring.Demo.Services
{
	public class OverviewService : IOverviewService
	{
		private readonly ServiceConfig _config;
		private readonly ILogger<OverviewService> _logger;
		private readonly ISolarEdgeHttpClient _solarEdgeHttpClient;
		private readonly IOverviewRepository _overviewRepository;
		private readonly IConverter<OverviewDto, Overview> _overviewConverter;

		public OverviewService(ILogger<OverviewService> logger,
			IOptions<ServiceConfig> config, ISolarEdgeHttpClient solarEdgeHttpClient,
			IOverviewRepository overviewRepository,
			IConverter<OverviewDto, Overview> overviewConverter)
		{
			_logger = logger;
			_config = config.Value;
			_solarEdgeHttpClient = solarEdgeHttpClient;
			_overviewRepository = overviewRepository;
			_overviewConverter = overviewConverter;
		}

		public async Task UpdateOverviewAsync(CancellationToken cancellationToken = default)
		{
			_logger.LogInformation($"Start reading Overview details at {DateTime.UtcNow.ToSqlDateTime()}");

			var overviewResult = await _solarEdgeHttpClient.GetOverviewInfoAsync(_config.SolarEdgeSiteId, cancellationToken).ConfigureAwait(false);
			var overview = _overviewConverter.Convert(overviewResult);

			// Add/update Overview table
			var existingOverview = await _overviewRepository.GetFirstAsync(cancellationToken).ConfigureAwait(false);
			if (existingOverview == null)
			{
				await _overviewRepository.AddAsync(overview, cancellationToken).ConfigureAwait(false);
			}
			else
			{
				overview.Id = existingOverview.Id;
				await _overviewRepository.UpdateAsync(overview, cancellationToken).ConfigureAwait(false);
			}
			_logger.LogInformation($"End reading Overview details at {DateTime.UtcNow.ToSqlDateTime()}");
		}

		public async Task<Overview> GetOverviewAsync(CancellationToken cancellationToken = default)
		{
			var overview = await _overviewRepository.GetFirstAsync(cancellationToken).ConfigureAwait(false);
			return overview;
		}
	}
}
