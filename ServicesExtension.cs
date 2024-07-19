using System.Security.Claims;
using System.Text;
using AuthWithControllersExample.Configuration;
using AuthWithControllersExample.Configuration.Policies;
using AuthWithControllersExample.Services;
using AuthWithControllersExample.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AuthWithControllersExample;

public static class ServicesExtension
{
    public static IServiceCollection AddJwt(this IServiceCollection services)
    {
        services.AddTransient<IJwtTokenGenerator, JwtTokenGenerator>(); // Добавляем наш самописный генератор токенов
        services.AddSingleton<IJwtTokensRepository, JwtTokensRepository>(); // Добавляем самописное хранилище токенов

        return services;
    }
    
    public static IServiceCollection AddConfiguredSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(); // Добавляем поддержку сваггера
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>(); // Добавить в сваггер кнопку авторизации

        return services;
    }
    
    public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
    {
        JwtSettings jwtSettings = new();
        configuration.Bind(nameof(JwtSettings), jwtSettings); // Достаем в объект настройки из конфига
        services.AddSingleton(Options.Create(jwtSettings)); // Регистрируем настройки из конфига через Options для доступа в других сервисах
        
        // Подключение аутентификации с указанием схемы
        services.AddAuthentication(defaultScheme: JwtBearerDefaults.AuthenticationScheme)
            // Добавляем аутентификацию через JwtBearer с настройкой параметров
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new()
                {
                    ValidateIssuer = true, // Проверять ли того, кто выдал токен
                    ValidateAudience = true, // Проверять ли того, кому выдан токен
                    ValidateLifetime = true, // Проверять ли время жизни токена
                    ValidateIssuerSigningKey = true, // Проверять ли секретный ключ
                    ValidIssuer = jwtSettings.Issuer, // Сервис, который отвечает за выдачу токена
                    ValidAudience = jwtSettings.Audience, // Сервис, которому можно выдавать токен
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)), // Секрет, который используется для шифрования
                };
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context => // Валидация токена
                    {
                        var tokensRepository =
                            context.HttpContext.RequestServices.GetRequiredService<IJwtTokensRepository>();
                        var userId = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                        if (userId is null // Если не нашил пользователя в контексте валидации, не пускаем
                            || context.SecurityToken.ValidTo < DateTime.UtcNow // Если токен истек, не пускаем
                            || !tokensRepository.Verify(int.Parse(userId), context.SecurityToken.UnsafeToString())) // Если токен не из нашей репы, не пускаем
                        {
                            context.Fail("Unauthorized");
                        }

                        return Task.CompletedTask;
                    }
                };
            });

        // Дополнительные настройки авторизации, если не хватает стандартной политики
        services.AddScoped<IAuthorizationHandler, NoteOwnerRequirementHandler>();
        services.AddAuthorization(options =>
        {
            var defaultAuthorizationPolicyBuilder =
                new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme);
            defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
            options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
            
            // Добавляем политику проверки пользователя из токена
            options.AddPolicy("NotesOwner", policy =>
            {
                policy.RequireAuthenticatedUser(); // Перед тем как проверять пользователя, отсекаем всех не аутентифицированных
                policy.AddRequirements(new NoteOwnerRequirement());
            });
        });

        return services;
    }
}