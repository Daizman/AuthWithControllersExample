using System.Collections.Concurrent;
using AuthWithControllersExample.Extensions;
using AuthWithControllersExample.Model;
using AuthWithControllersExample.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthWithControllersExample.Controllers;

public class UserController(IJwtTokenGenerator jwtTokenGenerator, IJwtTokensRepository jwtTokensRepository) 
    : BaseController
{
    private static readonly ConcurrentBag<User> Users = new();
    
    [AllowAnonymous] // Указывает, что этот метод доступен без авторизации
    [HttpPost("register")]
    public string Register(string name, string password)
    {
        if (Users.Any(user => user.Name == name))
        {
            throw new ArgumentException(nameof(name));
        }
        User user = new() { Id = Users.Count, Name = name, Password = password };
        Users.Add(user);
        var token = GenerateAndStoreToken(user);
        
        return token;
    }

    [AllowAnonymous] // Указывает, что этот метод доступен без авторизации
    [HttpPost("login")]
    public string Login(string name,[FromBody] string password)
    {
        var user = Users.FirstOrDefault(user => user.Name == name);
        if (user is null)
        {
            throw new ArgumentException(nameof(name));
        }

        if (user.Password != password)
        {
            throw new ArgumentException(nameof(password));
        }

        var token = GenerateAndStoreToken(user);

        return token;
    }

    [HttpDelete("logout")]
    public IActionResult Logout(int userId)
    {
        // Так как у нас есть репозиторий для хранения токенов, через который мы их проверяем, логаут можно сделать
        // через удаление токена из этого репозитория
        jwtTokensRepository.Remove(userId);

        return Ok();
    }

    [HttpGet("refreshtoken")]
    public string RefreshToken()
    {
        var userId = HttpContext.ExtractUserIdFromClaims();

        if (userId is null)
        {
            throw new InvalidOperationException();
        }

        var user = Users.FirstOrDefault(user => user.Id == userId);
        if (user is null)
        {
            throw new ArgumentException(nameof(userId));
        }

        var newToken = GenerateAndStoreToken(user);

        return newToken;
    }

    [HttpGet("allusers")]
    public List<string> GetAllUsers()
        => Users.Select(u => u.Name).ToList();
    
    
    private string GenerateAndStoreToken(User user)
    {
        var token = jwtTokenGenerator.GenerateToken(user);
        jwtTokensRepository.Update(user.Id, token);

        return token;
    }
}