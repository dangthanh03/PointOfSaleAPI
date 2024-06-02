namespace DoAnAPI.Models.Domain
{
    public class CartProduct
    {
        public int id { get; set; }
        public int CartId { get; set; }
        public Cart cart;
        public int ProductId { get; set; }
        public Product product;
        public int OrderQuanity { get; set; }
        public decimal? Price {  get; set; }

    }
}
