using System.Collections.Concurrent;

namespace AuthWithControllersExample.Services;

// Репозиторий токенов, позволяет преждевременно отзывать токены
public class JwtTokensRepository : IJwtTokensRepository
{
    // Потокобезопасная коллекция, так как могут быть обращения от разных запросов
    private readonly ConcurrentDictionary<int, string> _tokens = new();

    public void Update(int userId, string token) => _tokens[userId] = token;

    public bool Verify(int userId, string token) => _tokens.ContainsKey(userId) && _tokens[userId] == token;

    public void Remove(int userId) => _tokens.Remove(userId, out _);
}