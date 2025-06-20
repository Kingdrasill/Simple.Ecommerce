using Simple.Ecommerce.Api.Services.CacheServices;
using Simple.Ecommerce.Api.Services.ImageServices;
using Simple.Ecommerce.Domain.Settings.JwtSettings;
using Simple.Ecommerce.Domain.ValueObjects.UseCacheObject;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace Simple.Ecommerce.Api
{
    public static class DependecyInjection
    {
        public static IServiceCollection AddPresentation(
           this IServiceCollection services,
           IConfiguration configuration
        )
        {
            AddImageHostedService(services, configuration);

            AddCacheHostedService(services, configuration);

            AddJwtAuthetication(services, configuration);

            AddSwaggerGeneration(services, configuration);

            return services;
        }

        private static void AddImageHostedService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHostedService<CleanupImagesService>();
        }

        private static void AddCacheHostedService(this IServiceCollection services, IConfiguration configuration)
        {
            var cacheUse = configuration.GetSection("UseCache").Get<UseCache>()!;
            if (cacheUse.Use) { 
                services.AddHostedService<CacheCleanupService>();
                services.AddHostedService<CacheAheadService>();
            }
        }

        private static void AddJwtAuthetication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
            var key = Encoding.UTF8.GetBytes(jwtSettings!.SecretKey);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });
        }

        private static void AddSwaggerGeneration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header usando o esquema Bearer. Ex: 'Bearer {seu token}'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
        }
    }
}
