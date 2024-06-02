namespace DoAnAPI.Models.Domain
{
    public class Cart
    {
        public int CartId { get; set; }
        public string UserId { get; set; }

        public decimal Price { get; set; }
    }
}
