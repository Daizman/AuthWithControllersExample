using System.Security.Claims;

namespace AuthWithControllersExample.Extensions;

public static class HttpContextExtension
{
    // Расширение, которое ищеи в контексте запроса в пользовательских клеймах идентификатор пользователя и отдает его
    public static int? ExtractUserIdFromClaims(this HttpContext context)
    {
        var claim = context.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);
        return claim is null
            ? null
            : int.Parse(claim.Value);
    }
}