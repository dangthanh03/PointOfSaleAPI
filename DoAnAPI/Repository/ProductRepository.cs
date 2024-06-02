using DoAnAPI.IRepository;
using DoAnAPI.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace DoAnAPI.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly DatabaseContext _context;

        public ProductRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<Result<IEnumerable<Product>>> GetAllProducts(string searchTerm = "")
        {
            try
            {
                var products = await _context.Products.ToListAsync();
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    searchTerm = searchTerm.ToLower();
                    products = products.Where(a => a.Name.ToLower().StartsWith(searchTerm)).ToList();
                }
                return Result<IEnumerable<Product>>.Success(products);
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ và trả về kết quả thất bại
                return Result<IEnumerable<Product>>.Fail($"An error occurred while fetching products: {ex.Message}");
            }
        }

        public async Task<Result<Product>> GetProductById(int id)
        {
            try
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
                if (product != null)
                {
                    return Result<Product>.Success(product);
                }
                else
                {
                    return Result<Product>.Fail("Product not found.");
                }
            }
            catch (Exception ex)
            {
                var error = "Error: " + ex.Message;
                // Xử lý ngoại lệ khi truy vấn cơ sở dữ liệu
                return Result<Product>.Fail("An error occurred while fetching product information.");
            }
        }

        public async Task<Result<Product>> AddProduct(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return Result<Product>.Success(product);
        }

        public async Task<Result<Product>> UpdateProduct(Product product)
        {
            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Result<Product>.Success(product);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(product.Id))
                    return Result<Product>.Fail("Product not found.");
                else
                    throw;
            }
        }

        public async Task<Result<Product>> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return Result<Product>.Fail("Product not found.");

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return Result<Product>.Success(product);
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}

  