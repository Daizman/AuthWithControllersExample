using Microsoft.AspNetCore.Authorization;

namespace AuthWithControllersExample.Configuration.Policies;

// Необходимый класс, чтобы сделать кастомное условие
public class NoteOwnerRequirement : IAuthorizationRequirement;
