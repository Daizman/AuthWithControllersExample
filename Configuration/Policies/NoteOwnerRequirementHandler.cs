using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace AuthWithControllersExample.Configuration.Policies;

// Кастомное условие для авторизации на то, чтобы изменять заметку мог только пользователь, токен которого передан в
// заголовке
public class NoteOwnerRequirementHandler(IHttpContextAccessor accessor) : AuthorizationHandler<NoteOwnerRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        NoteOwnerRequirement requirement)
    {
        // Ищем в контексте у пользователя клейм с идентификатором
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim is null)
        {
            context.Fail();
            return Task.CompletedTask;
        }

        // Получаем httpContext запроса, чтобы достать из строки запроса userId
        var accessorResult = accessor.HttpContext!.Request.Query.TryGetValue("userId", out var userIdQuery);
        if (!accessorResult || !userIdQuery.Any())
        {
            context.Fail();
            return Task.CompletedTask;
        }
        
        // Проверяем, что заголок содержит того же пользователя, что и строка запроса
        if (userIdClaim.Value != userIdQuery.First())
        {
            context.Fail();
            return Task.CompletedTask;
        }
        
        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}