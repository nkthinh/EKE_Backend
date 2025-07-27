using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace EKE_Backend.Infrastructure
{
    public class FileUploadOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var fileParameters = context.MethodInfo.GetParameters()
                .Where(p => p.ParameterType == typeof(IFormFile) ||
                           p.ParameterType == typeof(IFormFile[]) ||
                           p.ParameterType == typeof(IEnumerable<IFormFile>))
                .ToList();

            if (!fileParameters.Any()) return;

            operation.RequestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["multipart/form-data"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties = fileParameters.ToDictionary(
                                p => p.Name ?? "file",
                                p => new OpenApiSchema
                                {
                                    Type = "string",
                                    Format = "binary"
                                }
                            ),
                            Required = fileParameters.Select(p => p.Name ?? "file").ToHashSet()
                        }
                    }
                }
            };
        }
    }
}