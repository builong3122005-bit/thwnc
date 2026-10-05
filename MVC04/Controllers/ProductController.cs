using Microsoft.AspNetCore.Mvc;
using MVC04.Models;
using MVC04.Repositories;

namespace MVC04.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepo;
        private readonly ProductModel _productModel;

        public ProductController(IProductRepository productRepo, ProductModel productModel)
        {
            _productRepo = productRepo;
            _productModel = productModel;
        }

        // GET: /Product/NewProduct
        [HttpGet]
        public IActionResult NewProduct()
        {
            return View();
        }

        // POST: /Product/NewProduct
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewProduct(Product product)
        {
            // Kiem tra rang buoc phia Controller
            if (string.IsNullOrWhiteSpace(product.ProductName))
            {
                ModelState.AddModelError(nameof(product.ProductName), "Tên mặt hàng không được để trống.");
            }
            else if (await _productRepo.IsProductNameExistsAsync(product.ProductName))
            {
                ModelState.AddModelError(nameof(product.ProductName), $"Tên mặt hàng '{product.ProductName}' đã tồn tại trong CSDL. Vui lòng chọn tên khác!");
            }

            if (string.IsNullOrWhiteSpace(product.ImageURL))
            {
                ModelState.AddModelError(nameof(product.ImageURL), "Vui lòng nhập URL ảnh đại diện.");
            }
            else if (!product.ImageURL.Trim().EndsWith(".png", StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(nameof(product.ImageURL), "URL ảnh đại diện phải có định dạng .png (kết thúc bằng đuôi .png).");
            }

            if (product.ProductPrice < 0)
            {
                ModelState.AddModelError(nameof(product.ProductPrice), "Đơn giá mặt hàng phải lớn hơn hoặc bằng 0.");
            }

            if (!ModelState.IsValid)
            {
                return View(product);
            }

            try
            {
                await _productRepo.AddAsync(product);
                await _productRepo.SaveAsync();

                TempData["SuccessMessage"] = $"Thêm mới mặt hàng \"{product.ProductName}\" thành công!";
                return RedirectToAction(nameof(NewProduct)); // Quay lai view NewProduct
            }
            catch (Exception ex)
            {
                var errorMsg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                ModelState.AddModelError(string.Empty, "Co loi khi luu vao CSDL: " + errorMsg);
                return View(product);
            }
        }

        // GET: /Product/ProductMgr - Hien danh sach mat hang
        [HttpGet]
        public async Task<IActionResult> ProductMgr()
        {
            var products = await _productRepo.GetAllAsync();
            return View(products);
        }

        // GET: /Product/Delete/{id} - Xoa mat hang va quay lai ProductMgr
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var isDeleted = await _productRepo.DeleteAsync(id);
            if (isDeleted)
            {
                await _productRepo.SaveAsync();
                TempData["SuccessMessage"] = "Xóa mặt hàng thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy mặt hàng cần xóa!";
            }

            return RedirectToAction(nameof(ProductMgr));
        }

        // GET: /Product/ProductList - Hien danh sach san pham theo dang luoi (Hinh 1)
        [HttpGet]
        public async Task<IActionResult> ProductList()
        {
            var products = await _productModel.GetProducts();
            return View(products);
        }

        // GET: /Product/ProductDetail/{id} - Hien chi tiet ve 1 mat hang
        [HttpGet]
        public async Task<IActionResult> ProductDetail(int id)
        {
            var product = await _productModel.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST /Product/DeleteAjax/{id} - Bai 5.1 xoa mat hang bang Ajax
        [HttpPost]
        [ActionName("DeleteAjax")]
        public async Task<IActionResult> DeleteAjax(int id)
        {
            try
            {
                var isDeleted = await _productRepo.DeleteAsync(id);
                if (isDeleted)
                {
                    await _productRepo.SaveAsync();
                    return Json(new { success = true, message = "Xóa mặt hàng thành công!" });
                }
                return Json(new { success = false, message = "Không tìm thấy mặt hàng cần xóa!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra khi xóa: " + ex.Message });
            }
        }

        // POST: /Product/Giamgia/{id} - Bài 5.2: Giảm giá 10% bằng AJAX
        [HttpPost]
        [ActionName("Giamgia")]
        public async Task<IActionResult> Giamgia(int id)
        {
            try
            {
                var product = await _productRepo.GetByIdAsync(id);
                if (product == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy mặt hàng cần giảm giá!" });
                }

                // Kiểm tra điều kiện: chỉ giảm khi giá >= 100.000 VNĐ
                if (product.ProductPrice >= 100000)
                {
                    // Giảm 10% (nhân 0.9)
                    product.ProductPrice = Math.Round(product.ProductPrice * 0.9m, 0);
                    await _productRepo.SaveAsync();

                    return Json(new
                    {
                        success = true,
                        message = $"Đã giảm giá 10% cho mặt hàng '{product.ProductName}'!",
                        newPrice = product.ProductPrice,
                        formattedPrice = string.Format("{0:N0} VND", product.ProductPrice),
                        canDiscountMore = product.ProductPrice >= 100000 // còn giảm được tiếp không
                    });
                }

                return Json(new
                {
                    success = false,
                    message = "Mặt hàng này có giá dưới 100,000 VND nên không được giảm giá nữa!"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi xử lý giảm giá: " + ex });
            }
        }

    }
}
