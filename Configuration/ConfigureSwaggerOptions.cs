using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AuthWithControllersExample.Configuration;

public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    // Настройка сваггера, чтобы в нем появилась кнопка авторизации, которая позволит записать токен
    // и передавать его в заголовке с каждым последующим запросом
    public void Configure(SwaggerGenOptions options)
    {
        options.AddSecurityDefinition(
            "Bearer", // Тип токена
            new OpenApiSecurityScheme // Схема для передачи токена в сваггере
            {
                In = ParameterLocation.Header, // Где будем передавать токен
                Description = "Please enter a valid token",
                Name = "Authorization", // Имя для заголовка
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT", // Формат для Bearer токена
                Scheme = JwtBearerDefaults.AuthenticationScheme
            });
        options.AddSecurityRequirement(
            new OpenApiSecurityRequirement
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
    }
}