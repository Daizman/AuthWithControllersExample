namespace AuthWithControllersExample.Services;

public interface IJwtTokensRepository
{
    void Update(int userId, string token);
    bool Verify(int userId, string token);
    void Remove(int userId);
}