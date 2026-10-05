using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC04.Models
{
    [Table("tblProducts")]
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Mã mặt hàng")]
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Tên mặt hàng không được để trống")]
        [StringLength(200, ErrorMessage = "Tên mặt hàng không được vượt quá 200 ký tự")]
        [Display(Name = "Tên mặt hàng")]
        public string ProductName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập URL ảnh đại diện")]
        [StringLength(500, ErrorMessage = "URL ảnh không được vượt quá 500 ký tự")]
        [RegularExpression(@"(?i)^.+\.png$", ErrorMessage = "Ảnh phải có định dạng .png (kết thúc bằng đuôi .png)")]
        [Display(Name = "URL ảnh đại diện (.png)")]
        public string? ImageURL { get; set; }

        [Required(ErrorMessage = "Đơn giá không được để trống")]
        [Range(0, 999999999999.99, ErrorMessage = "Đơn giá phải lớn hơn hoặc bằng 0")]
        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "Đơn giá")]
        public decimal ProductPrice { get; set; }

        [Display(Name = "Mô tả mặt hàng")]
        public string? Description { get; set; }
    }
}
