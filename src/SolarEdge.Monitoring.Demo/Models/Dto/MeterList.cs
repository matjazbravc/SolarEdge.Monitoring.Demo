namespace SolarEdge.Monitoring.Demo.Models.Dto
{
	public class MeterList
	{
		public string TimeUnit { get; set; }

		public string Unit { get; set; }

		public Meter[] Meters { get; set; }
	}
}
