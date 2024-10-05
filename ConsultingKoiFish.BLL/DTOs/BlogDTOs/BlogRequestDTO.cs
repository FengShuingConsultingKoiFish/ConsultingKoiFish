using System.ComponentModel.DataAnnotations;

namespace ConsultingKoiFish.BLL.DTOs.BlogDTOs;

public class BlogRequestDTO
{
    [Required(ErrorMessage = "Id không được để trống.")]
    public int Id { get; set; }
    [Required(ErrorMessage = "Tựa đề không được để trống.")]
    public string Title { get; set; } = null!;
    [Required(ErrorMessage = "Nội dung không được để trống.")]
    public string Content { get; set; } = null!;
    public List<int>? ImageIds { get; set; }
}