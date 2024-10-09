using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SolarEdge.Monitoring.Demo.Models;

[Table("EnergyDetails")]
public class EnergyDetails
{
  [Key]
  public int Id { get; set; }

  public DateTime Time { get; set; }

  public double SelfConsumption { get; set; }

  public double Consumption { get; set; }

  public double Purchased { get; set; }

  public double Production { get; set; }

  public double FeedIn { get; set; }
}
