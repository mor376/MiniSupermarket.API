using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniSupermarket.API.Data;
using MiniSupermarket.API.Models;

namespace MiniSupermarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly SupermarketDbContext _context;

        public ProductsController(SupermarketDbContext context)
        {
            _context = context;
        }

        // 1. GET: api/products - Lấy toàn bộ sản phẩm kèm Tên danh mục (Include)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products
                .Include(p => p.Category)
                .AsNoTracking()
                .ToListAsync();
        }

        // 2. GET: api/products/5 - Lấy chi tiết 1 sản phẩm theo ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound(new { message = "Không tìm thấy sản phẩm!" });
            }

            return product;
        }

        // 3. GET: api/products/barcode/{barcode} - Tra cứu sản phẩm theo mã vạch POS
        [HttpGet("barcode/{barcode}")]
        public async Task<ActionResult<Product>> GetProductByBarcode(string barcode)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Barcode == barcode);

            if (product == null)
            {
                return NotFound(new { message = "Không tìm thấy sản phẩm có mã vạch này!" });
            }

            return product;
        }

        // 4. POST: api/products - Thêm sản phẩm mới
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            product.Barcode = SupermarketDbContext.NormalizeProductCode(product.Barcode);
            product.Status = GetStatus(product.StockQuantity);
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = product.ProductId }, product);
        }

        // 5. PUT: api/products/5 - Cập nhật thông tin sản phẩm
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, Product product)
        {
            if (id != product.ProductId)
            {
                return BadRequest(new { message = "ID sản phẩm không khớp!" });
            }

            var existingProd = await _context.Products.FindAsync(id);
            if (existingProd == null)
            {
                return NotFound(new { message = "Không tìm thấy sản phẩm để cập nhật!" });
            }

            existingProd.Barcode = SupermarketDbContext.NormalizeProductCode(product.Barcode);
            existingProd.ProductName = product.ProductName;
            existingProd.CostPrice = product.CostPrice;
            existingProd.SellingPrice = product.SellingPrice;
            existingProd.StockQuantity = product.StockQuantity;
            existingProd.Status = GetStatus(product.StockQuantity);
            existingProd.Unit = product.Unit;
            existingProd.CategoryId = product.CategoryId;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // 6. DELETE: api/products/5 - Xóa sản phẩm
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(new { message = "Không tìm thấy sản phẩm để xóa!" });
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private static string GetStatus(int stockQuantity)
        {
            if (stockQuantity <= 5)
            {
                return "Cận hạn";
            }

            if (stockQuantity <= 15)
            {
                return "Sắp hết";
            }

            return "Bình thường";
        }
    }
}