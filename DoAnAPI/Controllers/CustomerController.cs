using DoAnAPI.IRepository;
using DoAnAPI.Models.Domain;
using DoAnAPI.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace DoAnAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [HttpGet]
        public async Task<ActionResult<Result<List<CustomerDTO>>>> GetCustomers()
        {
            var result = await _customerRepository.GetAll();
            if (result.IsSuccess)
                return Ok(result.Data);
            else
                return BadRequest(result.Message);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Result<CustomerDTO>>> GetCustomer(int id)
        {
            var result = await _customerRepository.GetById(id);
            if (result.IsSuccess)
                return Ok(result.Data);
            else
                return NotFound(result.Message);
        }

        [HttpPost]
        public async Task<ActionResult<Result<CustomerDTO>>> PostCustomer(CustomerDTO customer)
        {
            var result = await _customerRepository.Add(customer);
            if (result.IsSuccess)
                return Ok(result.Data);
            else
                return BadRequest(result.Message);
        }

        [HttpPut("AdjustCustomer")]
        public async Task<IActionResult> PutCustomer(Customer customer)
        {
            var result = await _customerRepository.Update( customer);
            if (result.IsSuccess)
                return NoContent();
            else
                return NotFound(result.Message);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var result = await _customerRepository.Delete(id);
            if (result.IsSuccess)
                return NoContent();
            else
                return NotFound(result.Message);
        }
    }
}
