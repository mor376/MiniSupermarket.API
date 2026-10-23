using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MiniSupermarket.API.Data;
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
        private readonly SupermarketDbContext _dbContext;

        public AuthController(IConfiguration configuration, SupermarketDbContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
        }

        // Endpoint Đăng nhập: POST /api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { success = false, message = "Tên đăng nhập và mật khẩu không được để trống." });
            }

            var user = await _dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == request.Username.Trim() && u.IsActive);

            if (user != null && PasswordHasher.Verify(request.Password, user.PasswordHash))
            {
                var token = GenerateJwtToken(user.Username, user.Role, user.FullName);
                return Ok(new { success = true, token, role = user.Role, fullName = user.FullName });
            }

            if (DemoAccounts.TryGetValue(request.Username.Trim(), out var role) && request.Password == "123456")
            {
                var token = GenerateJwtToken(request.Username.Trim(), role, request.Username.Trim());
                return Ok(new { success = true, token, role, fullName = request.Username.Trim() });
            }

            return Unauthorized(new { success = false, message = "Sai tài khoản hoặc mật khẩu!" });
        }

        private string GenerateJwtToken(string username, string role, string fullName)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:Secret"] ?? "SupermarketSecretKeyDoAnMonHoc2026SecureString!!");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.NameIdentifier, username),
                    new Claim(ClaimTypes.Role, role),
                    new Claim("fullName", fullName)
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
