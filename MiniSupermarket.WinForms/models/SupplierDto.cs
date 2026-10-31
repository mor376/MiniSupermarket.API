namespace MiniSupermarket.WinForms.Models
{
    public sealed class SupplierDto
    {
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public string? ContactName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
