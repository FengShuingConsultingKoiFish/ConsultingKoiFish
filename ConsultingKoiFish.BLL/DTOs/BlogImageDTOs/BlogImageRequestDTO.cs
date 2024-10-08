using System.ComponentModel.DataAnnotations;

namespace ConsultingKoiFish.BLL.DTOs.BlogImageDTOs;

public class BlogImageRequestDTO
{
    [Required(ErrorMessage = "BlogId không được để trống.")]
    public int BlogId { get; set; }

    public List<int>? ImagesId { get; set; } = new List<int>();
}