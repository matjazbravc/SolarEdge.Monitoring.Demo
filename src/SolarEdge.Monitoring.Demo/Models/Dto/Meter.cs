namespace SolarEdge.Monitoring.Demo.Models.Dto;

public class Meter
{
  public string Type { get; set; }

  public MeterValue[] Values { get; set; }
}
