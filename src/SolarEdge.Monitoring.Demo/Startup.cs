using HealthChecks.MySql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Quartz;
using SolarEdge.Monitoring.Demo.Database;
using SolarEdge.Monitoring.Demo.Extensions;
using SolarEdge.Monitoring.Demo.Models;
using SolarEdge.Monitoring.Demo.Models.Dto;
using SolarEdge.Monitoring.Demo.Services;
using SolarEdge.Monitoring.Demo.Services.Configuration;
using SolarEdge.Monitoring.Demo.Services.Converters;
using SolarEdge.Monitoring.Demo.Services.HttpClients;
using SolarEdge.Monitoring.Demo.Services.Polly;
using SolarEdge.Monitoring.Demo.Services.Quartz;
using SolarEdge.Monitoring.Demo.Services.Repositories;
using System;
using System.Threading.Tasks;

namespace SolarEdge.Monitoring.Demo;

public class Startup(IConfiguration configuration)
{
  public IConfiguration Configuration { get; } = configuration;

  public void ConfigureServices(IServiceCollection services)
  {
    services.AddOptions();

    // Register Swagger & Service configurations
    RegisterConfigurations(services);

    services.AddSingleton(Configuration);

    services.AddHttpClient("PollyHttpClient").AddPolicyHandler(RetryPolicies.GetHttpClientRetryPolicy());

    var mySqlConnString = Environment.GetEnvironmentVariable("ServiceConfig__MySqlConnectionString");
    if (string.IsNullOrEmpty(mySqlConnString))
    {
      // Read MySQL connection string from appsettings.json "ServiceConfig" section
      var config = Configuration.GetSection("ServiceConfig").Get<ServiceConfig>();
      mySqlConnString = config.MySqlConnectionString;
    }

    var options = new MySqlHealthCheckOptions(mySqlConnString);
    services.AddHealthChecks().AddMySql(options, "MySql", HealthStatus.Unhealthy);

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

    // Configure Quartz jobs
    services.AddQuartz(config =>
    {
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

  public static void Configure(IApplicationBuilder app, IWebHostEnvironment env, IConfiguration config, IDataInitializer dataInitializer)
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
      configure.MapDefaultControllerRoute();
      configure.MapHealthChecks("health");
      // Redirect root to Swagger UI
      configure.MapGet("", context =>
      {
        context.Response.Redirect("./swagger/index.html", permanent: false);
        return Task.CompletedTask;
      });
    });

    dataInitializer.InitializeAsync();
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
