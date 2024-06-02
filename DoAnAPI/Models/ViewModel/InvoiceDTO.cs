using DoAnAPI.Models.Domain;

namespace DoAnAPI.Models.ViewModel
{
    public class InvoiceDTO
    {
        public int? Id { get; set; }
        public DateTime Date { get; set; }
        public string UserId { get; set; }
        public decimal? Price { get; set; }
        public List<InvoiceProductDTO>? InvoiceProducts { get; set; }
    }
}
