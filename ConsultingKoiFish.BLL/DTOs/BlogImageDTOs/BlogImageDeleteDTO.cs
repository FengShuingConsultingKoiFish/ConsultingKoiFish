using System.ComponentModel.DataAnnotations;

namespace ConsultingKoiFish.BLL.DTOs.BlogImageDTOs;

public class BlogImageDeleteDTO
{
    [Required(ErrorMessage = "BlogId không thể để trống.")]
    public int BlogId { get; set; }
    [Required(ErrorMessage = "Vui lòng chọn ảnh muốn xóa.")]
    public List<int> BlogImageIds { get; set; } = new List<int>();
}