namespace DoAnAPI.Models.Domain
{
    public class InvoiceProduct
    {
        public int InvoiceId { get; set; }
        public virtual Invoice Invoice { get; set; }

        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
        public int Quantity { get; set; }
        public decimal? Price { get; set; }  
    }
}
