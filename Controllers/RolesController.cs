using Microsoft.AspNetCore.Mvc;
using MiniSupermarket.API.Models;

namespace MiniSupermarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        /// <summary>
        /// Dữ liệu mẫu lưu tạm trên bộ nhớ RAM để phục vụ kiểm thử (không dùng DB)
        /// </summary>
        private static readonly List<Role> _roles = new()
        {
            new Role { Id = 1, RoleName = "Quản trị viên", Description = "Quản trị viên toàn hệ thống, toàn quyền thao tác" },
            new Role { Id = 2, RoleName = "Thu ngân", Description = "Nhân viên thu ngân, chuyên trách bán hàng tại quầy" },
            new Role { Id = 3, RoleName = "Nhân viên kho", Description = "Nhân viên quản lý kho, nhập hàng và kiểm kê" }
        };

        /// <summary>
        /// 1. READ: Lấy toàn bộ danh sách vai trò (GET /api/roles)
        /// </summary>
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_roles);
        }

        /// <summary>
        /// 2. READ: Lấy chi tiết một vai trò theo ID (GET /api/roles/{id})
        /// </summary>
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var role = _roles.FirstOrDefault(r => r.Id == id);
            if (role == null)
            {
                return NotFound(new { message = "Không tìm thấy vai trò!" });
            }
            return Ok(role);
        }

        /// <summary>
        /// 3. CREATE: Thêm mới vai trò (POST /api/roles)
        /// </summary>
        [HttpPost]
        public IActionResult Create([FromBody] Role newRole)
        {
            if (string.IsNullOrWhiteSpace(newRole.RoleName))
            {
                return BadRequest(new { message = "Tên vai trò không được để trống!" });
            }

            // Tự động tăng ID
            newRole.Id = _roles.Count > 0 ? _roles.Max(r => r.Id) + 1 : 1;
            _roles.Add(newRole);

            return CreatedAtAction(nameof(GetById), new { id = newRole.Id }, newRole);
        }

        /// <summary>
        /// 4. UPDATE: Cập nhật thông tin vai trò (PUT /api/roles/{id})
        /// </summary>
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Role updateRole)
        {
            var role = _roles.FirstOrDefault(r => r.Id == id);
            if (role == null)
            {
                return NotFound(new { message = "Không tìm thấy vai trò cần sửa!" });
            }

            role.RoleName = updateRole.RoleName;
            role.Description = updateRole.Description;

            return NoContent();
        }

        /// <summary>
        /// 5. DELETE: Xóa vai trò theo ID (DELETE /api/roles/{id})
        /// </summary>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var role = _roles.FirstOrDefault(r => r.Id == id);
            if (role == null)
            {
                return NotFound(new { message = "Không tìm thấy vai trò cần xóa!" });
            }

            _roles.Remove(role);
            return NoContent();
        }
    }
}
