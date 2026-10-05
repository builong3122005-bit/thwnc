using MVC04.Repositories;

namespace MVC04.Models
{
    public class ProductModel
    {
        private readonly IProductRepository _productRepo;

        public ProductModel(IProductRepository productRepo)
        {
            _productRepo = productRepo;
        }

        // Phuong thuc GetProducts() theo yeu cau de bai
        public async Task<IEnumerable<Product>> GetProducts()
        {
            return await _productRepo.GetAllAsync();
        }

        // Phuong thuc lay chi tiet 1 san pham theo ID
        public async Task<Product?> GetProductById(int id)
        {
            return await _productRepo.GetByIdAsync(id);
        }
    }
}
