using DoAnAPI.IRepository;
using DoAnAPI.Models.Domain;
using Microsoft.AspNetCore.Mvc;

namespace DoAnAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IUserAuthenticateRepository _userAuthen;

        public ProductController(IProductRepository productRepository, IUserAuthenticateRepository userAuthen)
        {
            _productRepository = productRepository;
            _userAuthen = userAuthen;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts( string? searchTerm )
        {
            var products = await _productRepository.GetAllProducts(searchTerm);
            return Ok(products.Data);
        }


        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _productRepository.GetProductById(id);
            if (product == null)
                return NotFound();
            return Ok(product.Data);
        }

        // POST: api/Products
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {

            var result = await _productRepository.AddProduct(product);
            if (!result.IsSuccess)
                return BadRequest(result.Message);
            return CreatedAtAction(nameof(GetProduct), new { id = result.Data.Id }, result.Data);
        }

        // PUT: api/Products/5
        [HttpPut("AdjustProduct")]
        public async Task<IActionResult> PutProduct(Product product)
        {
            if (product == null)
                return BadRequest("Invalid product data.");

            var result = await _productRepository.UpdateProduct(product);
            if (!result.IsSuccess)
                return NotFound(result.Message);

            return NoContent();
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _productRepository.DeleteProduct(id);
            if (!result.IsSuccess)
                return NotFound(result.Message);
            return NoContent();
        }
    }
}
