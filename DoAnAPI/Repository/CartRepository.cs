using DoAnAPI.IRepository;
using DoAnAPI.Models.Domain;
using DoAnAPI.Models.ViewModel;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace DoAnAPI.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly IProductRepository _productRepository;
        private readonly DatabaseContext _context;
        private readonly IInvoiceRepository _invoiceRepository;

        public CartRepository(DatabaseContext context, IProductRepository productRepository, IInvoiceRepository invoiceRepository)
        {
            _context = context;
            _productRepository = productRepository;
            _invoiceRepository = invoiceRepository;
        }
        public async Task<Result<string>> AddProductToCartAsync(int cartId, int ProductId, int quanity)
        {
            try
            {
                var productDto = await _productRepository.GetProductById(ProductId);
                if (!productDto.IsSuccess)
                {
                    return Result<string>.Fail("Cannot find product to add to cart");

                }
                // Kiểm tra xem có bản ghi CartShoe nào tương ứng chưa
                var existingCartProduct = await _context.CartProducts
                    .FirstOrDefaultAsync(cs => cs.CartId == cartId && cs.ProductId == ProductId);

                if (existingCartProduct != null)
                {
                    existingCartProduct.OrderQuanity += quanity;
                    existingCartProduct.Price +=  (productDto.Data.Price * quanity);


                }
                else
                {
                   
                    // Nếu chưa có, tạo một bản ghi mới
                    CartProduct newCartProduct = new CartProduct
                    {
                        CartId = cartId,
                        ProductId = ProductId,
                        OrderQuanity = quanity,
                        Price = productDto.Data.Price*quanity

                        // Các thuộc tính khác của CartShoe nếu có
                    };

                    _context.CartProducts.Add(newCartProduct);
                }

                // Lưu các thay đổi vào cơ sở dữ liệu
                await _context.SaveChangesAsync();

                return Result<string>.Success("Product added to cart successfully");
            }
            catch (Exception ex)
            {
                return Result<string>.Fail($"Error adding shoe to cart: {ex.Message}");
            }
        }



        public async Task<Result<string>> CreateCartAsync(string userId)
        {
            try
            {
                Cart cart = new Cart
                {
                    UserId = userId,
                    // Các thuộc tính khác của giỏ hàng nếu có
                };

                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();

                return Result<string>.Success("Cart created successfully");
            }
            catch (Exception ex)
            {
                return Result<string>.Fail($"Error creating cart: {ex.Message}");
            }
        }
        public async Task<Result<int>> GetCartAsync(string userId)
        {
            try
            {
                var result = _context.Carts.FirstOrDefault(c => c.UserId == userId);

                return Result<int>.Success(result.CartId);

            }
            catch (Exception ex)
            {
                return Result<int>.Fail("No cart exists: " + ex.Message);
            }
        }

        public async Task<Result<string>> RemoveFromCart(int productId, string userId)
        {
            try
            {
                var cart = await GetCartAsync(userId);
                if (cart.IsSuccess)
                {

                    var result = (from cartProduct in _context.CartProducts

                                  where cartProduct.CartId == cart.Data && cartProduct.ProductId == productId
                                  select cartProduct).FirstOrDefault();

                    if (result != null)
                    {
                       

                       
                        _context.CartProducts.Remove(result); // Xóa bản ghi từ bộ nhớ của Entity Framework
                        await _context.SaveChangesAsync(); // Lưu thay đổi vào cơ sở dữ liệu
                    }
                }

                else
                {
                    return Result<string>.Fail("Cart does not exist");

                }
                return Result<string>.Success("Product removed from cart successfully.");
            }
            catch (Exception ex)
            {
                // Nếu có lỗi xảy ra trong quá trình xóa sản phẩm khỏi giỏ hàng
                // Bạn có thể ghi log và trả về kết quả thất bại cùng với thông báo lỗi
                return Result<string>.Fail("An error occurred while removing product from cart.");
            }
        }

        public async Task<Result<int>> BuyAsync(string userId)
        {
            try
            {
                // Get cart items by user ID
                var cart = await GetCartAsync(userId);

                // Ensure cart exists
                if (!cart.IsSuccess || cart.Data == null)
                {
                    return Result<int>.Fail("Cart not found.");
                }
                var productIds = await GetProductIdsFromCartProduct(cart.Data);
                // Ensure cart exists
                if (!productIds.IsSuccess || productIds.Data == null)
                {
                    return Result<int>.Fail("Cart is empty");
                }
                var DetailCart = await GetProductAsync(cart.Data);
                if (!DetailCart.IsSuccess || DetailCart.Data == null)
                {
                    return Result<int>.Fail("fetching CartProduct fail");
                }

                var totalAmount = await CalculateTotalAmountAsync(DetailCart.Data);
                // Create invoice
                var invoice = new Invoice
                {
                    UserId = userId,
                    Date = DateTime.UtcNow,
                    // Calculate total amount based on cart items or product prices
                    TotalAmount = totalAmount.Data
                };

                // Save invoice to database
                var invoiceResult = await _invoiceRepository.CreateInvoiceAsync(invoice, DetailCart.Data);
                if (!invoiceResult.IsSuccess)
                {
                    return Result<int>.Fail("Failed to create invoice.");
                }

                // Clear cart after successful purchase
                var clearCartResult = await ClearCartAsync(cart.Data);
                if (!clearCartResult.IsSuccess)
                {
                    return Result<int>.Fail("Failed to clear cart after purchase.");
                }

                return Result<int>.Success(invoiceResult.Data.Id);
            }
            catch (Exception ex)
            {
                // Log exception if needed
                return Result<int>.Fail("Failed to complete purchase: " + ex.Message);
            }
        }

        private async Task<Result<decimal>> CalculateTotalAmountAsync(List<ProductCartVm> productCarts)
        {
            try
            {
                decimal totalPrice = 0;
                foreach (var productCart in productCarts)
                {
                    totalPrice += (decimal)productCart.Price;
                }
                return Result<decimal>.Success(totalPrice);
            }
            catch (Exception ex)
            {
                // Log exception if needed
                return Result<decimal>.Fail("Failed to calculate total amount: " + ex.Message);
            }
        }
        public async Task<Result<string>> ClearCartAsync(int cartId)
        {
            try
            {
                // Find cart items by cart ID
                var cartItems = _context.CartProducts.Where(item => item.CartId == cartId);

                // Remove cart items from database
                _context.CartProducts.RemoveRange(cartItems);

                // Save changes to the database
                await _context.SaveChangesAsync();

                return Result<string>.Success("Cart cleared successfully.");
            }
            catch (Exception ex)
            {
                // Log exception if needed
                return Result<string>.Fail("Failed to clear cart: " + ex.Message);
            }
        }
        public async Task<Result<List<int>>> GetProductIdsFromCartProduct(int cartId)
        {
            try
            {
                // Query cart shoes by cart ID
                var cartProducts = await _context.CartProducts
                    .Where(cs => cs.CartId == cartId)
                    .ToListAsync();

                // Extract product IDs from cart shoes
                var productIds = cartProducts.Select(cs => cs.ProductId).ToList();

                return Result<List<int>>.Success(productIds);
            }
            catch (Exception ex)
            {
                // Log exception if needed
                // Return error result with message
                return Result<List<int>>.Fail("Failed to retrieve product IDs from cart shoes: " + ex.Message);
            }
        }

        public async Task<Result<List<ProductCartVm>>> GetProductAsync(int cartId)
        {
            try
            {
                var products = (from cart in _context.Carts
                                join cartProducts in _context.CartProducts
                                on cart.CartId equals cartProducts.CartId
                                join product in _context.Products
                                on cartProducts.ProductId equals product.Id
                                where cart.CartId == cartId
                                select cartProducts
                               ).ToList();

                var ProductList = new List<ProductCartVm>();
                foreach (var product in products)
                {
                    // Gọi phương thức GetProductById để lấy thông tin sản phẩm
                    var productResult = await _productRepository.GetProductById(product.ProductId);

                    // Kiểm tra kết quả trả về từ GetProductById
                    if (productResult.IsSuccess)
                    {
                        var productInCart = productResult.Data;

                        var productCartVm = new ProductCartVm
                        {
                            ProductId = product.ProductId,
                            OrderQuanity = product.OrderQuanity,
                            product = productInCart,
                            Price = product.Price,
                        };

                        ProductList.Add(productCartVm);
                    }
                    else
                    {
                        // Nếu không tìm thấy sản phẩm, xử lý lỗi
                        return Result<List<ProductCartVm>>.Fail("Failed to get product with ID " + product.ProductId);
                    }
                }

                return Result<List<ProductCartVm>>.Success(ProductList);
            }
            catch (Exception ex)
            {
                return Result<List<ProductCartVm>>.Fail("Fail to get product via cart id : " + ex.Message);
            }
        }


    }
}
