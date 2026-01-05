using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using CleanApi.Api.Models;

namespace Api.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;

        public AuthController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("token")]
        public async Task<IActionResult> GenerateToken(TokenRequest request)
        {
            Console.WriteLine("---- AUTH CONTROLLER HIT ----");
            Console.WriteLine($"Username: '{request.Username}'");
            Console.WriteLine($"Password: '{request.Password}'");

            var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString();
            if (string.IsNullOrEmpty(clientIp))
                return BadRequest("Client IP not available");

            // Open SQL connection
            using var conn = new SqlConnection(_config.GetConnectionString("Default"));
            await conn.OpenAsync();
            Console.WriteLine($"CONNECTED DB: {conn.Database}");

            // Use VarChar for password to match HASHBYTES correctly
            var cmd = new SqlCommand(@"
                SELECT Id
                FROM APICredentials
                WHERE Username = @u
                  AND PasswordHash = HASHBYTES('SHA2_256', @p)
                  AND IsActive = 1", conn);

            cmd.Parameters.Add("@u", SqlDbType.NVarChar, 200).Value = request.Username;
            cmd.Parameters.Add("@p", SqlDbType.VarChar, 100).Value = request.Password;

            var credentialId = (int?)await cmd.ExecuteScalarAsync();
            Console.WriteLine($"RAW RESULT: {credentialId}");

            if (credentialId == null)
                return Unauthorized();

            // Generate token
            var token = Guid.NewGuid();
            var expiry = DateTime.UtcNow.AddHours(1);
            Console.WriteLine($"token: '{token}'");
            Console.WriteLine($"expiry: '{expiry}'");

            // Insert token into DB
            var insert = new SqlCommand(@"
                INSERT INTO APITokens (CredentialId, Token, Expiry, ClientIp)
                VALUES (@cid, @t, @e, @ip)", conn);

            insert.Parameters.Add("@cid", SqlDbType.Int).Value = credentialId.Value;
            insert.Parameters.Add("@t", SqlDbType.UniqueIdentifier).Value = token;
            insert.Parameters.Add("@e", SqlDbType.DateTime2).Value = expiry;
            insert.Parameters.Add("@ip", SqlDbType.NVarChar, 50).Value = clientIp;

            await insert.ExecuteNonQueryAsync();

            return Ok(new { token, expiry });
        }
    }
}
