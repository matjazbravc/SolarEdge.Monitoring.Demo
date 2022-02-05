namespace SolarEdge.Monitoring.Demo.Services.Configuration
{
	public class ServiceConfig
	{
		public string SolarEdgeSiteId { get; set; }

		public string SolarEdgeApiKey { get; set; }

		public string MySqlConnectionString { get; set; }

		public string OverviewJobCronSchedule { get; set; }
		
		public string EnergyDetailsJobCronSchedule { get; set; }
	}
}
