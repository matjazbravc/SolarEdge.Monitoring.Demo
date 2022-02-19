using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;
using System.Text.Json;

namespace SolarEdge.Monitoring.Demo.Services.Swagger.Filters;

/// <summary>
/// Represents the Swagger/Swashbuckle operation filter used to document the implicit API version parameter.
/// </summary>
/// <remarks>This <see cref="IOperationFilter"/> is only required due to bugs in the <see cref="SwaggerGenerator"/>.
/// Once they are fixed and published, this class can be removed.</remarks>
public class SwaggerDefaultValues : IOperationFilter
{
  /// <summary>
  /// Applies the filter to the specified operation using the given context.
  /// </summary>
  /// <param name="operation">The operation to apply the filter to.</param>
  /// <param name="context">The current operation filter context.</param>
  public void Apply(OpenApiOperation operation, OperationFilterContext context)
  {
    ApiDescription apiDescription = context.ApiDescription;
    operation.Deprecated |= apiDescription.IsDeprecated();

    foreach (ApiResponseType responseType in context.ApiDescription.SupportedResponseTypes)
    {
      string responseKey = responseType.IsDefaultResponse ? "default" : responseType.StatusCode.ToString();
      OpenApiResponse response = operation.Responses[responseKey];

      foreach (string contentType in response.Content.Keys)
      {
        if (responseType.ApiResponseFormats.All(x => x.MediaType != contentType))
        {
          response.Content.Remove(contentType);
        }
      }
    }

    if (operation.Parameters == null)
    {
      return;
    }

    foreach (OpenApiParameter parameter in operation.Parameters)
    {
      ApiParameterDescription description = apiDescription.ParameterDescriptions.First(p => p.Name == parameter.Name);
      parameter.Description ??= description.ModelMetadata.Description;
      if (parameter.Schema.Default == null && description.DefaultValue != null)
      {
        string json = JsonSerializer.Serialize(description.DefaultValue, description.ModelMetadata.ModelType);
        parameter.Schema.Default = OpenApiAnyFactory.CreateFromJson(json);
      }
      parameter.Required |= description.IsRequired;
    }
  }
}