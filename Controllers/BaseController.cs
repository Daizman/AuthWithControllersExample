using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthWithControllersExample.Controllers;

// Базовый контроллер, чтобы не прописывать все атрибуты у других контроллеров снова
[ApiController]
[Route("api/[controller]")]
[Authorize] // Атрибут, указывающий, что все методы в контроллере доступны только авторизованным пользователям
public abstract class BaseController : Controller;
