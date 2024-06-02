using DoAnAPI.Models.Domain;
using DoAnAPI.Models.ViewModel;

namespace DoAnAPI.IRepository
{
    public interface IInvoiceRepository
    {
        Task<Result<Invoice>> CreateInvoiceAsync(Invoice invoice, List<ProductCartVm> productIds);
        Task<Result<List<InvoiceDTO>>> GetAllInvoices();
        Task<Result<InvoiceDTO>> GetInvoiceById(int? id);
        Task<Result<InvoiceDTO>> AddInvoice(Invoice invoice);
        Task<Result<Invoice>> UpdateInvoice(Invoice invoice);
        Task<Result<InvoiceDTO>> DeleteInvoice(int id);
        Task<Result<List<InvoiceDTO>>> GetInvoicesByCustomerId(string customerId);
    }
}
