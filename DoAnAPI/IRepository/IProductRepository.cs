using DoAnAPI.Models.Domain;

namespace DoAnAPI.IRepository
{
    public interface IProductRepository
    {
        Task<Result<IEnumerable<Product>>> GetAllProducts(string searchTerm);
        Task<Result<Product>> GetProductById(int id);
        Task<Result<Product>> AddProduct(Product product);
        Task<Result<Product>> UpdateProduct(Product product);
        Task<Result<Product>> DeleteProduct(int id);
    }
}
