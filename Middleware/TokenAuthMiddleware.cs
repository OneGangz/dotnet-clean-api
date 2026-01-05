using Microsoft.Data.SqlClient;
public class TokenAuthMiddleware
{
    private readonly RequestDelegate _next;

    public TokenAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IConfiguration config)
    {Console.WriteLine("---- MIDDLEWARE HIT ----");
Console.WriteLine($"Path: {context.Request.Path}");

        var path = context.Request.Path.Value?.ToLower();
        if (path!.StartsWith("/swagger") || path.StartsWith("/auth") || path == "/")
        {
            await _next(context);    
            return;
        }
        if (context.Request.Path.StartsWithSegments("/auth"))
        {
            await _next(context);
            return;
        }

        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        Console.WriteLine($"PATH: {context.Request.Path}");
Console.WriteLine($"AUTH HEADER: {authHeader}");
        if (authHeader == null || !authHeader.StartsWith("Bearer "))
        {
            context.Response.StatusCode = 401;
            return;
        }

        if (!Guid.TryParse(authHeader.Substring(7), out var token))
        {
            context.Response.StatusCode = 401;
            return;
        }

        var clientIp = context.Connection.RemoteIpAddress?.ToString();
        if (string.IsNullOrEmpty(clientIp))
        {
            context.Response.StatusCode = 401;
            return;
        }

        using var conn = new SqlConnection(config.GetConnectionString("Default"));
        await conn.OpenAsync();

        var cmd = new SqlCommand(@"
            SELECT 1
            FROM APITokens
            WHERE Token = @t
              AND ClientIp = @ip
              AND Expiry > SYSUTCDATETIME()", conn);

        cmd.Parameters.AddWithValue("@t", token);
        cmd.Parameters.AddWithValue("@ip", clientIp);

        var valid = await cmd.ExecuteScalarAsync();
        if (valid == null)
        {
            context.Response.StatusCode = 401;
            return;
        }

        await _next(context);
    }
}
