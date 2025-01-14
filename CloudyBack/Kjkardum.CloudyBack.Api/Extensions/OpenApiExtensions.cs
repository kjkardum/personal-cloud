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
                "Bearer",
                new OpenApiSecurityScheme
                {
                    Description = """
                                  JWT Authorization token using the Bearer scheme. <br>
                                  Enter 'Bearer' [space] and then your token in the text input below. <br>
                                  Example: 'Bearer 12345abcdef'
                                  """,
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
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
        });

        return services;
    }
}
