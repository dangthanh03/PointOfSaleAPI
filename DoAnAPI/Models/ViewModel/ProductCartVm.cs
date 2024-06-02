using DoAnAPI.Models.Domain;

namespace DoAnAPI.Models.ViewModel
{
    public class ProductCartVm
    {
        
        public int ProductId { get; set; }
        public Product product { get; set; }
        public int OrderQuanity { get; set; }
        public decimal? Price { get; set; }

    }
}
