using System.ComponentModel.DataAnnotations;

namespace MiniSupermarket.API.Models
{
    public class CartItemCheckoutDto
    {
        [Range(1, int.MaxValue)]
        public int ProductId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        public decimal? UnitPrice { get; set; }
    }

    public class CheckoutRequestDto
    {
        [Range(1, int.MaxValue)]
        public int CustomerId { get; set; }

        [Required]
        [MinLength(1)]
        public List<CartItemCheckoutDto> Items { get; set; } = new();
    }
}
