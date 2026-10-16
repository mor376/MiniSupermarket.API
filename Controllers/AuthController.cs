using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MiniSupermarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private static readonly IReadOnlyDictionary<string, string> DemoAccounts =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["admin01"] = "Admin",
                ["admin02"] = "Admin",
                ["admin_backup"] = "Admin",
                ["supervisor"] = "Admin",
                ["cashier01"] = "Cashier",
                ["cashier02"] = "Cashier",
                ["cashier03"] = "Cashier",
                ["cashier04"] = "Cashier",
                ["cashier05"] = "Cashier",
                ["cashier06"] = "Cashier",
                ["ware01"] = "Warehouse",
                ["ware02"] = "Warehouse",
                ["ware03"] = "Warehouse",
                ["ware04"] = "Warehouse",
                ["ware05"] = "Warehouse",
                ["admin"] = "Admin",
                ["cashier"] = "Cashier"
            };

        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Endpoint Đăng nhập: POST /api/auth/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequestDto request)
        {
            if (!string.IsNullOrWhiteSpace(request.Username)
                && request.Password == "123456"
                && DemoAccounts.TryGetValue(request.Username, out var role))
            {
                var token = GenerateJwtToken(request.Username, role);
                return Ok(new { success = true, token, role });
            }

            return Unauthorized(new { success = false, message = "Sai tài khoản hoặc mật khẩu!" });
        }

        private string GenerateJwtToken(string username, string role)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:Secret"] ?? "SupermarketSecretKeyDoAnMonHoc2026SecureString!!");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, role)
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

    public class LoginRequestDto
    {
        [System.ComponentModel.DataAnnotations.Required]
        public string Username { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Required]
        public string Password { get; set; } = string.Empty;
    }
}
