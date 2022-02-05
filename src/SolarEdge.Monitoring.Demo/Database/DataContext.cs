using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SolarEdge.Monitoring.Demo.Models;
using SolarEdge.Monitoring.Demo.Services.Configuration;

namespace SolarEdge.Monitoring.Demo.Database
{
	/// <summary>
	/// Database context
	/// </summary>
	public class DataContext : DbContext
	{
		private readonly ServiceConfig _config;

		public DataContext(IOptions<ServiceConfig> config)
		{
			_config = config.Value;
		}
		
		public DbSet<EnergyDetails> EnergyDetails { get; set; }
		
		public DbSet<Overview> Overview { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder options)
		{
			var connectionString = _config.MySqlConnectionString;
			options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
				.EnableSensitiveDataLogging()
				.EnableDetailedErrors();
		}
	}
}
