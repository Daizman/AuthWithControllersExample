using AuthWithControllersExample.Model;

namespace AuthWithControllersExample.Services;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}