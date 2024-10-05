using System.ComponentModel.DataAnnotations;

namespace ConsultingKoiFish.BLL.DTOs.BlogImageDTOs;

public class BlogImageCreateDTO
{
    [Required(ErrorMessage = "BlogId không được để trống.")]
    public int BlogId { get; set; }
    [Required(ErrorMessage = "BlogId không được để trống.")]
    public int ImageId { get; set; }
}