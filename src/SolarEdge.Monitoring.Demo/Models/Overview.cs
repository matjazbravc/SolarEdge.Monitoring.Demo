using System;
using System.ComponentModel.DataAnnotations;

namespace SolarEdge.Monitoring.Demo.Models;

/// <summary>
/// Site overview
/// </summary>
public class Overview
{
  [Key]
  public int Id { get; set; }

  public DateTime Time { get; set; }

  public string Metric { get; set; }

  public double LifeTimeData { get; set; }

  public double LastYearData { get; set; }

  public double LastMonthData { get; set; }

  public double LastDayData { get; set; }

  public double CurrentPower { get; set; }
}
