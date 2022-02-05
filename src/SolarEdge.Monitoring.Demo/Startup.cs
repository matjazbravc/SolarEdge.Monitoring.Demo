using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Quartz;
using SolarEdge.Monitoring.Demo.Database;
using SolarEdge.Monitoring.Demo.Extensions;
using SolarEdge.Monitoring.Demo.Models.Dto;
using SolarEdge.Monitoring.Demo.Models;
using SolarEdge.Monitoring.Demo.Services.Configuration;
using SolarEdge.Monitoring.Demo.Services.Converters;
using SolarEdge.Monitoring.Demo.Services.HttpClients;
using SolarEdge.Monitoring.Demo.Services.Polly;
using SolarEdge.Monitoring.Demo.Services.Quartz;
using SolarEdge.Monitoring.Demo.Services.Repositories;
using SolarEdge.Monitoring.Demo.Services;
using System;

namespace SolarEdge.Monitoring.Demo
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddOptions();

			RegisterConfigurations(services);

			services.AddSingleton(Configuration);

			services.AddHttpClient("PollyHttpClient").AddPolicyHandler(RetryPolicies.GetHttpClientRetryPolicy());

			var mySqlConnString = Environment.GetEnvironmentVariable("ServiceConfig__MySqlConnectionString");
			if (string.IsNullOrEmpty(mySqlConnString))
			{
				var config = Configuration.GetSection("ServiceConfig").Get<ServiceConfig>();
				mySqlConnString = config.MySqlConnectionString;
			}

			services.AddHealthChecks()
				.AddMySql(mySqlConnString, "MySql", HealthStatus.Unhealthy);

			// Register converters
			services.AddTransient<IConverter<OverviewDto, Overview>, OverviewResultToOverviewConverter>();
			services.AddTransient<IConverter<EnergyDetailsDto, EnergyDetails>, EnergyDetailsDtoToEnergyDetailsConverter>();

			// Register services
			services.AddTransient<IDataInitializer, DataInitializer>();
			services.AddTransient<IEnergyDetailsRepository, EnergyDetailsRepository>();
			services.AddTransient<IEnergyDetailsService, EnergyDetailsService>();
			services.AddTransient<IOverviewRepository, OverviewRepository>();
			services.AddTransient<IOverviewService, OverviewService>();
			services.AddTransient<ISolarEdgeHttpClient, SolarEdgeHttpClient>();

			services.AddQuartz(config =>
			{
				config.UseMicrosoftDependencyInjectionJobFactory();

				// Create a keys for the jobs
				var solarEdgeGetOverviewJobKey = new JobKey(nameof(SolarEdgeGetOverviewJob));
				var solarEdgeGetEnergyDetailsJobKey = new JobKey(nameof(SolarEdgeGetEnergyDetailsJob));

				// Register the job with the DI container
				config.AddJob<SolarEdgeGetOverviewJob>(opts => opts.WithIdentity(solarEdgeGetOverviewJobKey));
				config.AddJob<SolarEdgeGetEnergyDetailsJob>(opts => opts.WithIdentity(solarEdgeGetEnergyDetailsJobKey));

				// Create a trigger for SolarEdgeGetOverviewJob
				var overviewJobCronSchedule = Environment.GetEnvironmentVariable("ServiceConfig__OverviewJobCronSchedule");
				if (string.IsNullOrEmpty(overviewJobCronSchedule))
				{
					overviewJobCronSchedule = Configuration.GetSection("ServiceConfig").Get<ServiceConfig>().OverviewJobCronSchedule;
				}
				config.AddTrigger(configure => configure
					.ForJob(solarEdgeGetOverviewJobKey)
					.WithIdentity($"{nameof(SolarEdgeGetOverviewJob)}-Trigger")
					.WithCronSchedule(overviewJobCronSchedule, x => x.InTimeZone(TimeZoneInfo.Local))); // https://www.freeformatter.com/cron-expression-generator-quartz.html

				// Create a trigger for SolarEdgeGetEnergyDetailsJob
				var energyDetailsJobCronSchedule = Environment.GetEnvironmentVariable("ServiceConfig__EnergyDetailsJobCronSchedule");
				if (string.IsNullOrEmpty(energyDetailsJobCronSchedule))
				{
					energyDetailsJobCronSchedule = Configuration.GetSection("ServiceConfig").Get<ServiceConfig>().EnergyDetailsJobCronSchedule;
				}
				config.AddTrigger(configure => configure
					.ForJob(solarEdgeGetEnergyDetailsJobKey)
					.WithIdentity($"{nameof(SolarEdgeGetEnergyDetailsJob)}-Trigger")
					.WithCronSchedule(energyDetailsJobCronSchedule, x => x.InTimeZone(TimeZoneInfo.Local)));
			});
			
			services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

			services.AddCorsPolicy("EnableCORS");
			services.AddAndConfigureApiVersioning();
			services.AddDbContext<DataContext>();
			services.AddHttpContextAccessor();
			
			services.AddControllers()
				.ConfigureApiBehaviorOptions(options =>
				{
					options.SuppressConsumesConstraintForFormFileParameters = true;
					options.SuppressInferBindingSourcesForParameters = true;
					options.SuppressModelStateInvalidFilter = true;
					options.SuppressMapClientErrors = true;
					options.ClientErrorMapping[404].Link = "https://httpstatuses.com/404";
				})
				.AddNewtonsoftJson(options =>
				{
					options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
				});

			services.AddRouting(options => options.LowercaseUrls = true);
			services.AddSwaggerMiddleware();
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IConfiguration config, IDataInitializer dataInitializer)
		{
			app.UseApiExceptionHandling();
			app.UseSwaggerMiddleware(config);

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseHsts();
			}

			app.UseHsts();
			app.UseRouting();
			app.UseCors("EnableCORS");
			app.UseStaticFiles();

			app.UseEndpoints(configure =>
			{
				configure.MapControllers();
				configure.MapGet("/", async context =>
				{
					await context.Response.WriteAsync("SolarEdge.Monitoring.OpenApi");
				});
				configure.MapHealthChecks("health");
				configure.MapDefaultControllerRoute();
			});

			dataInitializer.Initialize().Wait();
		}

		/// <summary>
		/// Register a configuration instances which TOptions will bind against
		/// </summary>
		/// <param name="services"></param>
		protected void RegisterConfigurations(IServiceCollection services)
		{
			services.Configure<ServiceConfig>(Configuration.GetSection(nameof(ServiceConfig)));
			services.Configure<SwaggerConfig>(Configuration.GetSection(nameof(SwaggerConfig)));
		}
	}
}
