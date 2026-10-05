using MVC04.Models;

namespace MVC04.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
        Task<bool> IsProductNameExistsAsync(string productName, int? excludeId = null);
        Task AddAsync(Product product);
        Task<bool> DeleteAsync(int id);
        Task<int> SaveAsync();
    }
}
