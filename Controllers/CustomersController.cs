using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniSupermarket.API.Data;
using MiniSupermarket.API.Models;

namespace MiniSupermarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly SupermarketDbContext _context;

        public CustomersController(SupermarketDbContext context)
        {
            _context = context;
        }

        // 1. GET: api/customers (Lấy danh sách tất cả khách hàng)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            return await _context.Customers.ToListAsync();
        }

        // 2. GET: api/customers/5 (Lấy chi tiết 1 khách hàng theo ID)
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound("Không tìm thấy khách hàng!");
            }

            return customer;
        }

        // 3. GET: api/customers/search?keyword=... (Tìm kiếm theo tên hoặc SĐT)
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Customer>>> SearchCustomers([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return await _context.Customers.ToListAsync();
            }

            keyword = keyword.Trim().ToLower();

            return await _context.Customers
                .Where(c => c.CustomerName.ToLower().Contains(keyword) ||
                            c.PhoneNumber.Contains(keyword))
                .ToListAsync();
        }

        // 4. POST: api/customers (Thêm mới khách hàng)
        [HttpPost]
        public async Task<ActionResult<Customer>> CreateCustomer([FromBody] Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            customer.PhoneNumber = customer.PhoneNumber.Trim();
            customer.Status = NormalizeStatus(customer.Status);

            if (await _context.Customers.AnyAsync(c => c.PhoneNumber == customer.PhoneNumber))
            {
                return Conflict(new { message = $"Số điện thoại '{customer.PhoneNumber}' đã tồn tại trong hệ thống." });
            }

            try
            {
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetCustomer), new { id = customer.CustomerId }, customer);
            }
            catch (DbUpdateException)
            {
                return Conflict(new { message = "Số điện thoại đã tồn tại. Vui lòng kiểm tra lại thông tin khách hàng." });
            }
        }

        // 5. PUT: api/customers/5 (Cập nhật thông tin khách hàng)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody] Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return BadRequest("ID khách hàng không trùng khớp!");
            }

            customer.PhoneNumber = customer.PhoneNumber.Trim();
            customer.Status = NormalizeStatus(customer.Status);

            var duplicateCustomer = await _context.Customers
                .FirstOrDefaultAsync(c => c.PhoneNumber == customer.PhoneNumber && c.CustomerId != id);

            if (duplicateCustomer != null)
            {
                return Conflict(new { message = $"Số điện thoại '{customer.PhoneNumber}' đã tồn tại cho khách hàng khác." });
            }

            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
                {
                    return NotFound("Khách hàng không tồn tại!");
                }
                throw;
            }
            catch (DbUpdateException)
            {
                return Conflict(new { message = "Số điện thoại đã tồn tại. Vui lòng kiểm tra lại thông tin khách hàng." });
            }

            return NoContent();
        }

        // 6. DELETE: api/customers/5 (Xóa khách hàng)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound("Khách hàng không tồn tại!");
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Xóa khách hàng thành công!" });
        }

        private static string NormalizeStatus(string? status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                return "Hoạt động";
            }

            var value = status.Trim();
            if (value.Equals("Hoạt động", StringComparison.OrdinalIgnoreCase)
                || value.Equals("Hoat dong", StringComparison.OrdinalIgnoreCase)
                || value.Equals("Active", StringComparison.OrdinalIgnoreCase)
                || value.Equals("Đang hoạt động", StringComparison.OrdinalIgnoreCase))
            {
                return "Hoạt động";
            }

            if (value.Equals("Khóa", StringComparison.OrdinalIgnoreCase)
                || value.Equals("Khoa", StringComparison.OrdinalIgnoreCase)
                || value.Equals("Locked", StringComparison.OrdinalIgnoreCase)
                || value.Equals("Inactive", StringComparison.OrdinalIgnoreCase)
                || value.Equals("Không hoạt động", StringComparison.OrdinalIgnoreCase))
            {
                return "Khóa";
            }

            return value;
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }
    }
}