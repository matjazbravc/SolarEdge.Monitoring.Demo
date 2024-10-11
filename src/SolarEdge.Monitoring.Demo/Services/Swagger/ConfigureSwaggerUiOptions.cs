using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using SolarEdge.Monitoring.Demo.Services.Configuration;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Linq;

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
    foreach (var groupName in _apiProvider.ApiVersionDescriptions.Select(description => description.GroupName))
    {
      options.SwaggerEndpoint($"/{_swaggerConfig.RoutePrefix}/{groupName}/{_swaggerConfig.DocsFile}", groupName);
    }
  }
}
