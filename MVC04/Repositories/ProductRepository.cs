using Microsoft.EntityFrameworkCore;
using MVC04.Data;
using MVC04.Models;

namespace MVC04.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products.AsNoTracking().ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<bool> IsProductNameExistsAsync(string productName, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(productName))
                return false;

            var trimmedName = productName.Trim().ToLower();

            if (excludeId.HasValue)
            {
                return await _context.Products
                    .AnyAsync(p => p.ProductID != excludeId.Value && p.ProductName.ToLower() == trimmedName);
            }

            return await _context.Products
                .AnyAsync(p => p.ProductName.ToLower() == trimmedName);
        }

        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
        }

        // Xoa san pham theo ID
        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return false;

            _context.Products.Remove(product);
            return true;
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
