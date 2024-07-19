namespace AuthWithControllersExample.Settings;

public class JwtSettings
{
    // Название приложения, которое будет подписывать токен
    public string Issuer { get; set; } = null!;
    // Название приложения, для которого подписываем токен
    public string Audience { get; set; } = null!;
    // Секретная фраза для шифрования
    public string Secret { get; set; } = null!;
}