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

namespace SolarEdge.Monitoring.Demo.Services;

	public class OverviewService(
		ILogger<OverviewService> logger,
  IOptions<ServiceConfig> config,
		ISolarEdgeHttpClient solarEdgeHttpClient,
  IOverviewRepository overviewRepository,
		IConverter<OverviewDto, Overview> overviewConverter)
		: IOverviewService
	{
		private readonly ServiceConfig _config = config.Value;

  public async Task UpdateOverviewAsync(CancellationToken cancellationToken = default)
		{
			logger.LogInformation($"Start reading Overview details at {DateTime.UtcNow.ToSqlDateTime()}");

			var overviewResult = await solarEdgeHttpClient.GetOverviewInfoAsync(_config.SolarEdgeSiteId, cancellationToken).ConfigureAwait(false);
			var overview = overviewConverter.Convert(overviewResult);

			// Add/update Overview table
			var existingOverview = await overviewRepository.GetFirstAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
			if (existingOverview == null)
			{
				await overviewRepository.AddAsync(overview, cancellationToken).ConfigureAwait(false);
			}
			else
			{
				overview.Id = existingOverview.Id;
				await overviewRepository.UpdateAsync(overview, cancellationToken: cancellationToken).ConfigureAwait(false);
			}
			logger.LogInformation($"End reading Overview details at {DateTime.UtcNow.ToSqlDateTime()}");
		}

		public async Task<Overview> GetOverviewAsync(CancellationToken cancellationToken = default)
		{
			var overview = await overviewRepository.GetFirstAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
			return overview;
		}
	}
