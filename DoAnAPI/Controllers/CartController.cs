using DoAnAPI.IRepository;
using DoAnAPI.Models.Domain;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;

namespace DoAnAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly IInvoiceRepository invoiceRepository;
        private readonly ICartRepository cartRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        public CartController(UserManager<ApplicationUser> userManager, IInvoiceRepository invoiceRepository, ICartRepository cartRepository)
        {
            this.cartRepository = cartRepository;
            _userManager = userManager;

            this.invoiceRepository = invoiceRepository;

        }

        [HttpPost("AddToCart/{ProductId}")]
        public async Task<IActionResult> AddToCart(int ProductId, int quantity,string userId)
        {
           
            var cartResult = await cartRepository.GetCartAsync(userId);
            if (!cartResult.IsSuccess)
            {
                return BadRequest($"Error getting user's cart: {cartResult.Message}");
            }

            var cart = cartResult.Data;

            var addProductResult = await cartRepository.AddProductToCartAsync(cart, ProductId, quantity);
            if (!addProductResult.IsSuccess)
            {
                return BadRequest($"Error adding product to cart: {addProductResult.Message}");
            }

            var cartProductsResult = await cartRepository.GetProductAsync(cart);
            if (!cartProductsResult.IsSuccess)
            {
                return BadRequest($"Error getting products in cart: {cartProductsResult.Message}");
            }

            var cartProducts = cartProductsResult.Data;
            return Ok(cartProducts);
        }
        [HttpGet("CheckCart")]
        public async Task<IActionResult> CheckCart(string Id)
        {
            var cartResult = await cartRepository.GetCartAsync(Id);
            if (!cartResult.IsSuccess)
            {
                return NotFound("Cart not found.");
            }

            var cart = cartResult.Data;

            var result = await cartRepository.GetProductAsync(cart);
            if (!result.IsSuccess)
            {
                return NotFound("Products not found in the cart.");
            }

            return Ok(result.Data);
        }


        [HttpPost("RemoveFromCart/{ProductId}")]
        public async Task<IActionResult> RemoveFromCart(int ProductId, string userId)
        {

            var result = await cartRepository.RemoveFromCart(ProductId, userId);
            if (result.IsSuccess) {
                return Ok(result.Data);
            }
            else
            {
                return BadRequest(result.Data);
            }
        }

        [HttpPost("Buy")]
        public async Task<IActionResult> Buy(string userId)
        {
        
            var result = await cartRepository.BuyAsync(userId);
            if (result.IsSuccess)
            {
                var invoice = await invoiceRepository.GetInvoiceById(result.Data);

                return Ok(invoice.Data);

            }
            else
            {
                return BadRequest(new { ErrorMessage = "Failed to complete purchase." });
            }


        }


    }
}
