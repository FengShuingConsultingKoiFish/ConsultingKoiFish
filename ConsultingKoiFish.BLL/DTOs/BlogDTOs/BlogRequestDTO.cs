using System.ComponentModel.DataAnnotations;
using ConsultingKoiFish.DAL.Enums;

namespace ConsultingKoiFish.BLL.DTOs.BlogDTOs;

public class BlogRequestDTO
{
    [Required(ErrorMessage = "Id không được để trống.")]
    public int Id { get; set; }
    [Required(ErrorMessage = "Tựa đề không được để trống.")]
    public string Title { get; set; } = null!;
    [Required(ErrorMessage = "Nội dung không được để trống.")]
    public string Content { get; set; } = null!;

    public BlogStatus? Status { get; set; }
    public List<int>? ImageIds { get; set; }

    public bool IsStatusValid()
    {
        if (this.Status.HasValue)
        {
            return Enum.IsDefined(typeof(BlogStatus), Status);    
        }
        
    }
}