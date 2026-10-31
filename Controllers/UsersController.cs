using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniSupermarket.API.Data;
using MiniSupermarket.API.Models;

namespace MiniSupermarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly SupermarketDbContext _dbContext;

        public UsersController(SupermarketDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var users = await _dbContext.Users
                .AsNoTracking()
                .OrderBy(u => u.UserId)
                .Select(u => new UserListItemDto
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    FullName = u.FullName,
                    Role = u.Role,
                    IsActive = u.IsActive
                })
                .ToListAsync(cancellationToken);

            return Ok(users);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserId == id, cancellationToken);

            if (user == null)
            {
                return NotFound(new { message = "Không tìm thấy người dùng." });
            }

            return Ok(new UserDetailDto
            {
                UserId = user.UserId,
                Username = user.Username,
                FullName = user.FullName,
                Role = user.Role,
                IsActive = user.IsActive
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.FullName) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { message = "Tên đăng nhập, họ tên và mật khẩu không được để trống." });
            }

            var normalizedUsername = request.Username.Trim();
            var exists = await _dbContext.Users.AnyAsync(u => u.Username == normalizedUsername, cancellationToken);
            if (exists)
            {
                return Conflict(new { message = "Tên đăng nhập đã tồn tại." });
            }

            var user = new User
            {
                Username = normalizedUsername,
                FullName = request.FullName.Trim(),
                Role = string.IsNullOrWhiteSpace(request.Role) ? "Cashier" : request.Role.Trim(),
                PasswordHash = PasswordHasher.Hash(request.Password),
                IsActive = request.IsActive
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Ok(new { userId = user.UserId, message = "Tạo tài khoản thành công." });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == id, cancellationToken);
            if (user == null)
            {
                return NotFound(new { message = "Không tìm thấy người dùng để cập nhật." });
            }

            if (!string.IsNullOrWhiteSpace(request.FullName))
            {
                user.FullName = request.FullName.Trim();
            }

            if (!string.IsNullOrWhiteSpace(request.Role))
            {
                user.Role = request.Role.Trim();
            }

            if (request.IsActive.HasValue)
            {
                user.IsActive = request.IsActive.Value;
            }

            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                user.PasswordHash = PasswordHasher.Hash(request.Password);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            return Ok(new { message = "Cập nhật người dùng thành công." });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == id, cancellationToken);
            if (user == null)
            {
                return NotFound(new { message = "Không tìm thấy người dùng để xóa." });
            }

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return Ok(new { message = "Xóa người dùng thành công." });
        }
    }

    public class CreateUserRequest
    {
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = "Cashier";
        public bool IsActive { get; set; } = true;
    }

    public class UpdateUserRequest
    {
        public string? FullName { get; set; }
        public string? Role { get; set; }
        public string? Password { get; set; }
        public bool? IsActive { get; set; }
    }

    public class UserListItemDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class UserDetailDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
