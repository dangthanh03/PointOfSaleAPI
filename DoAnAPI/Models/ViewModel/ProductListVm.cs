using DoAnAPI.Models.Domain;

namespace DoAnAPI.Models.ViewModel
{
    public class ProductListVm
    {
        public string? SearchTerm { get; set; }
        public List<Product> Products { get; set; }
    }
}
