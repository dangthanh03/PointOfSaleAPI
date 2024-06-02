namespace DoAnAPI.Models.Domain
{
    public class Invoice
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string UserId {  get; set; }
       
        public virtual List<InvoiceProduct>? InvoiceProducts { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
