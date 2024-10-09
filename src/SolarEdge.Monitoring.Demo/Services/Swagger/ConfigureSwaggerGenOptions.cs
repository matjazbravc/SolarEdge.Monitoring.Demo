using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using SolarEdge.Monitoring.Demo.Services.Configuration;
using SolarEdge.Monitoring.Demo.Services.Swagger.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.IO;
using System.Reflection;
using System;

namespace SolarEdge.Monitoring.Demo.Services.Swagger;

/// <summary>
/// Configures the Swagger generation options
/// </summary>
/// <remarks>This allows API versioning to define a Swagger document per API version after the
/// <see cref="IApiVersionDescriptionProvider"/> service has been resolved from the service container.</remarks>
/// <param name="apiProvider">The <see cref="IApiVersionDescriptionProvider">apiProvider</see> used to generate Swagger documents.</param>
/// <param name="swaggerConfig"></param>
public class ConfigureSwaggerGenOptions(
  IApiVersionDescriptionProvider apiProvider,
  IOptions<SwaggerConfig> swaggerConfig)
  : IConfigureOptions<SwaggerGenOptions>
{
  private readonly SwaggerConfig _swaggerConfig = swaggerConfig.Value;
  private readonly IApiVersionDescriptionProvider _apiProvider = apiProvider ?? throw new ArgumentNullException(nameof(apiProvider));

  /// <inheritdoc />
  public void Configure(SwaggerGenOptions options)
  {
    // Add a custom operation filter which sets default values
    options.OperationFilter<SwaggerDefaultValues>();

    // Add a swagger document for each discovered API version
    // Note: you might choose to skip or document deprecated API versions differently
    foreach (var description in _apiProvider.ApiVersionDescriptions)
    {
      options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));

      // Include Document file
      var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
      var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
      options.IncludeXmlComments(xmlPath);

      // Provide a custom strategy for generating the unique Id's
      options.CustomSchemaIds(x => x.FullName);
    }
  }

  /// <summary>
  /// Create API version
  /// </summary>
  /// <param name="description"></param>
  /// <returns></returns>
  private OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
  {
    var info = new OpenApiInfo()
    {
      Title = _swaggerConfig.Title,
      Version = description.ApiVersion.ToString(),
      Description = _swaggerConfig.Description,
      Contact = new OpenApiContact
      {
        Name = _swaggerConfig.ContactName,
        Email = _swaggerConfig.ContactEmail,
        Url = new Uri(_swaggerConfig.ContactUrl)
      },
      License = new OpenApiLicense
      {
        Name = _swaggerConfig.LicenseName,
        Url = new Uri(_swaggerConfig.LicenseUrl)
      }
    };

    if (description.IsDeprecated)
    {
      info.Description += " ** THIS API VERSION HAS BEEN DEPRECATED!";
    }

    return info;
  }
}