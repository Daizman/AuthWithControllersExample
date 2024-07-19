using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthWithControllersExample.Model;
using AuthWithControllersExample.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AuthWithControllersExample.Services;

// Сервис генерации токенов
public class JwtTokenGenerator(IOptions<JwtSettings> jwtSettings) : IJwtTokenGenerator
{
    public string GenerateToken(User user)
    {
        var options = jwtSettings.Value;
        
        // Настройки подписи токена
        SigningCredentials signingCredentials = new(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Secret)),
            SecurityAlgorithms.HmacSha256);

        // Клеймы
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.GivenName, user.Name),
            new Claim(JwtRegisteredClaimNames.FamilyName, user.Name),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };
        // Время жизни
        var expiration = DateTime.UtcNow.AddMinutes(5);

        JwtSecurityToken securityToken = new(
            issuer: options.Issuer, // Сервис, который подписал токен
            audience: options.Audience, // Сервис, для которого подписан токен
            expires: expiration,
            claims: claims,
            signingCredentials: signingCredentials
        );

        var token = new JwtSecurityTokenHandler().WriteToken(securityToken);

        return token;
    }
}