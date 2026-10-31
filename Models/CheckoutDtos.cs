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

<<<<<<< HEAD
        public bool UseRewardPoints { get; set; }

        [Range(0, 50)]
        public int RewardPointsToUse { get; set; }

=======
>>>>>>> f0301efe1787856d53ccf540414a58191753b22f
        [Required]
        [MinLength(1)]
        public List<CartItemCheckoutDto> Items { get; set; } = new();
    }
}
