using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using SolarEdge.Monitoring.Demo.Middleware;
using SolarEdge.Monitoring.Demo.Services.Configuration;

namespace SolarEdge.Monitoring.Demo.Extensions;

/// <summary>
/// Application extensions
/// </summary>
public static class AppExtensions
{
  /// <summary>
  /// Adds global exception handling middleware
  /// </summary>
  /// <param name="app"></param>
  public static IApplicationBuilder UseApiExceptionHandling(this IApplicationBuilder app) => app.UseMiddleware<ApiExceptionHandlingMiddleware>();

  /// <summary>
  /// Register Swagger and SwaggerUI middleware
  /// </summary>
  /// <param name="app"></param>
  /// <param name="config"></param>
  public static void UseSwaggerMiddleware(this IApplicationBuilder app, IConfiguration config)
  {
    var swaggerConfig = config.GetSection(nameof(SwaggerConfig)).Get<SwaggerConfig>();
    app.UseSwagger(options =>
    {
      options.RouteTemplate = $"{swaggerConfig.RoutePrefix}/{{documentName}}/{swaggerConfig.DocsFile}";
    });
    app.UseSwaggerUI();
  }
}
