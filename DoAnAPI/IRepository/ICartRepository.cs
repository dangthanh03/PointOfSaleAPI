using DoAnAPI.Models.Domain;
using DoAnAPI.Models.ViewModel;

namespace DoAnAPI.IRepository
{
    public interface ICartRepository
    {
        Task<Result<string>> CreateCartAsync(string userId);
        Task<Result<string>> AddProductToCartAsync(int cartId, int shoeId, int quanity);
        Task<Result<int>> GetCartAsync(string userId);
        Task<Result<List<ProductCartVm>>> GetProductAsync(int cartId);
        Task<Result<string>> RemoveFromCart(int ProductId, string userId);
        Task<Result<int>> BuyAsync(string userId);
    }
}
