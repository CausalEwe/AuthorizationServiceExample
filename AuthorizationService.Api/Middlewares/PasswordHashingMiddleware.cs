using System.Text;
using System.Text.Json;

namespace AuthorizationService.Api.Middlewares;

public class PasswordHashingMiddleware
{
    private readonly RequestDelegate _next;

    public PasswordHashingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Method == HttpMethod.Post.ToString())
        {
            if (context.Request.Path.StartsWithSegments("/api/auth/register"))
            {
                var originalBody = context.Request.Body;
                var requestBody = await new StreamReader(originalBody).ReadToEndAsync();

                var data = JsonSerializer.Deserialize<Dictionary<string, string>>(requestBody);

                if (data != null && data.ContainsKey("password"))
                {
                    var password = data["password"];
                    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

                    data["password"] = hashedPassword;

                    var modifiedRequestBody = JsonSerializer.Serialize(data);

                    var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(modifiedRequestBody));
                    context.Request.Body = memoryStream;

                    context.Request.Body.Seek(0, SeekOrigin.Begin);
                }
            }
        }

        await _next(context);
    }
}