using System;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SolarEdge.Monitoring.Demo.Models.Dto;
using SolarEdge.Monitoring.Demo.Services.Configuration;

namespace SolarEdge.Monitoring.Demo.Services.HttpClients;

public class SolarEdgeHttpClient(
  IOptions<ServiceConfig> config,
  IHttpClientFactory clientFactory)
  : ISolarEdgeHttpClient
{
  private readonly Uri _baseUri = new("https://monitoringapi.solaredge.com/");
  private readonly ServiceConfig _config = config.Value;

  public async Task<EnergyDetailsDto> GetEnergyDetailsAsync(string siteId, DateTime start, DateTime end, CancellationToken cancellationToken = default)
  {
    return await SendGetRequest<EnergyDetailsDto>($"site/{siteId}/energyDetails.json?timeUnit=DAY&startTime={ConvertToString(start)}&endTime={ConvertToString(end)}", cancellationToken).ConfigureAwait(false);
  }

  public async Task<OverviewDto> GetOverviewInfoAsync(string siteId, CancellationToken cancellationToken = default)
  {
    return await SendGetRequest<OverviewDto>($"site/{siteId}/overview?", cancellationToken).ConfigureAwait(false);
  }

  public async Task<PowerDetailsDto> GetPowerDetailsAsync(string siteId, DateTime start, DateTime end, CancellationToken cancellationToken = default)
  {
    return await SendGetRequest<PowerDetailsDto>($"site/{siteId}/powerDetails.json?timeUnit=QUARTER_OF_AN_HOUR&startTime={ConvertToString(start)}&endTime={ConvertToString(end)}", cancellationToken).ConfigureAwait(false);
  }

  private static TData Convert<TData>(string content)
  {
    var jsonSettings = new JsonSerializerSettings
    {
      MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
      DateParseHandling = DateParseHandling.None,
    };

    var dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
    jsonSettings.Converters.Add(dateTimeConverter);

    return JsonConvert.DeserializeObject<TData>(content, jsonSettings);
  }

  private static string ConvertToString(DateTime time) => time.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

  private async Task<TData> SendGetRequest<TData>(string relativePath, CancellationToken cancellationToken = default)
  {
    var httpClient = clientFactory.CreateClient("PollyHttpClient");
    var uri = new UriBuilder(new Uri(_baseUri, relativePath));
    var apiKey = $"&api_key={_config.SolarEdgeApiKey}";
    if (string.IsNullOrEmpty(uri.Query))
    {
      uri.Query = apiKey;
    }
    else
    {
      uri.Query += apiKey;
    }
    var apiResult = await httpClient.GetAsync(uri.Uri, cancellationToken).ConfigureAwait(false);
    var strContent = await apiResult.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
    var result = cancellationToken.IsCancellationRequested ?
      default :
      Convert<TData>(strContent);
    return result;
  }
}
