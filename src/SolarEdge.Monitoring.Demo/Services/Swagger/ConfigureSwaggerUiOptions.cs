using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using SolarEdge.Monitoring.Demo.Services.Configuration;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;

namespace SolarEdge.Monitoring.Demo.Services.Swagger;

/// <summary>
/// Configures the Swagger UI options
/// </summary>
/// <param name="apiProvider">The API provider.</param>
/// <param name="swaggerConfig"></param>
public class ConfigureSwaggerUiOptions(
  IApiVersionDescriptionProvider apiProvider,
  IOptions<SwaggerConfig> swaggerConfig)
  : IConfigureOptions<SwaggerUIOptions>
{
  private readonly SwaggerConfig _swaggerConfig = swaggerConfig.Value;
  private readonly IApiVersionDescriptionProvider _apiProvider = apiProvider ?? throw new ArgumentNullException(nameof(apiProvider));

  /// <inheritdoc />
  public void Configure(SwaggerUIOptions options)
  {
    options = options ?? throw new ArgumentNullException(nameof(options));
    options.RoutePrefix = _swaggerConfig.RoutePrefix;
    options.DocumentTitle = _swaggerConfig.Description;
    options.DocExpansion(DocExpansion.List);
    options.DefaultModelExpandDepth(0);

    // Configure Swagger JSON endpoints
    foreach (var description in _apiProvider.ApiVersionDescriptions)
    {
      options.SwaggerEndpoint($"/{_swaggerConfig.RoutePrefix}/{description.GroupName}/{_swaggerConfig.DocsFile}", description.GroupName);
    }
  }
}
