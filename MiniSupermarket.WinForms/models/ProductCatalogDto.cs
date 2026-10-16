namespace MiniSupermarket.WinForms.Models
{
    public sealed class ProductCatalogDto
    {
        public int ProductId { get; set; }
        public string Barcode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal CostPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string Unit { get; set; } = "Cái";
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
}
