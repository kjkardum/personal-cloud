using Microsoft.OpenApi.Models;
using System.Reflection;

namespace Kjkardum.CloudyBack.Api.Extensions;

public static class OpenApiExtensions
{
    public static IServiceCollection AddSwagger(
        this IServiceCollection services)
    {
        services.AddSwaggerGen(setup =>
        {
            setup.EnableAnnotations();

            setup.AddSecurityDefinition(
                "Jwt Token",
                new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization token. Enter your token in the text input below.",
                    Name = "x-cloudy-token",
                    In = ParameterLocation.Cookie,
                    Type = SecuritySchemeType.ApiKey,
                });

            setup.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Authorization",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });
            setup.SupportNonNullableReferenceTypes();
            var currentAssembly = Assembly.GetExecutingAssembly();
            var xmlDocs = currentAssembly.GetReferencedAssemblies()
                .Union([currentAssembly.GetName()])
                .Select(a => Path.Combine(Path.GetDirectoryName(currentAssembly.Location)!, $"{a.Name}.xml"))
                .Where(File.Exists)
                .ToArray();

            Array.ForEach(xmlDocs, (d) => setup.IncludeXmlComments(d));
        });

        return services;
    }
}
