using DoAnAPI.IRepository;
using DoAnAPI.Models.Domain;
using DoAnAPI.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace DoAnAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceRepository _invoiceRepository;

        public InvoiceController(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        // POST: api/Invoice
      
        // GET: api/Invoice
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Invoice>>> GetInvoices()
        {
            var invoices = await _invoiceRepository.GetAllInvoices();
           
            return Ok(invoices.Data);
        }

        // GET: api/Invoice/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Invoice>> GetInvoice(int id)
        {
            var invoice = await _invoiceRepository.GetInvoiceById(id);
            if (!invoice.IsSuccess)
                return NotFound(invoice.Message);
            return Ok(invoice.Data);
        }

      
      

        // PUT: api/Invoices/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInvoice(int id, Invoice invoice)
        {
            if (id != invoice.Id)
                return BadRequest("Invalid invoice ID.");
            var result = await _invoiceRepository.UpdateInvoice(invoice);
            if (!result.IsSuccess)
                return NotFound(result.Message);
            return NoContent();
        }

        // DELETE: api/Invoice/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            var result = await _invoiceRepository.DeleteInvoice(id);
            if (!result.IsSuccess)
                return NotFound(result.Message);
            return Ok(result.Data);
        }
        
        [HttpGet("User/{UserId}")]
        public async Task<ActionResult<Result<List<InvoiceDTO>>>> GetInvoicesByCustomerId(string UserId)
        {
            var result = await _invoiceRepository.GetInvoicesByCustomerId(UserId);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
    }
}
