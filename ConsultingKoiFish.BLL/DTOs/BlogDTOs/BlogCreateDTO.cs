using System.ComponentModel.DataAnnotations;

namespace ConsultingKoiFish.BLL.DTOs.BlogDTOs;

public class BlogCreateDTO
{
    [Required(ErrorMessage = "Tựa đề không được để trống.")]
    public string Title { get; set; } = null!;
    [Required(ErrorMessage = "Nội dung không được để trống.")]
    public string Content { get; set; } = null!;
}