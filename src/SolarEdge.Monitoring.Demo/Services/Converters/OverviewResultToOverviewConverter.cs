using Microsoft.Extensions.Logging;
using SolarEdge.Monitoring.Demo.Models;
using SolarEdge.Monitoring.Demo.Models.Dto;

namespace SolarEdge.Monitoring.Demo.Services.Converters;

/// <summary>
/// OverviewDto to Overview converter
/// </summary>
public class OverviewResultToOverviewConverter(
  ILogger<OverviewResultToOverviewConverter> logger)
  : IConverter<OverviewDto, Overview>
{
  public Overview Convert(OverviewDto overviewDto)
  {
    logger.LogDebug(nameof(Convert));

    var result = new Overview
    {
      CurrentPower = overviewDto.Overview.CurrentPower.Power,
      LastDayData = overviewDto.Overview.LastDayData.Energy,
      LastMonthData = overviewDto.Overview.LastMonthData.Energy,
      LastYearData = overviewDto.Overview.LastYearData.Energy,
      LifeTimeData = overviewDto.Overview.LifeTimeData.Energy,
      Metric = overviewDto.Overview.MeasuredBy,
      Time = overviewDto.Overview.LastUpdateTime
    };

    return result;
  }
}