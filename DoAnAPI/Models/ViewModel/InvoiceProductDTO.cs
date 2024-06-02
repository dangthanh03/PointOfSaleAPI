using DoAnAPI.Models.Domain;

namespace DoAnAPI.Models.ViewModel
{
    public class InvoiceProductDTO
    {
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        public decimal? Price { get; set; }

        public int Quantity { get; set; }
    }
}
