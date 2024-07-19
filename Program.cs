using AuthWithControllersExample;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddHttpContextAccessor()
    .AddConfiguredSwagger()
    .AddJwt()
    .AddAuth(builder.Configuration)
    .AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication(); // Сначала аутентификация
app.UseAuthorization(); // Потом авторизация
app.MapControllers(); // И только потом контроллеры

app.Run();
