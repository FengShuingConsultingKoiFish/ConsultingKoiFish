using System.ComponentModel.DataAnnotations;
using ConsultingKoiFish.DAL.Enums;

namespace ConsultingKoiFish.BLL.DTOs.BlogDTOs;

public class BlogUpdateStatusDTO
{
    [Required(ErrorMessage = "Id không được để trống.")]
    public int Id { get; set; }
    [Required(ErrorMessage = "Status không được để trống.")]
    public BlogStatus? Status { get; set; }

    public bool IsStatusValid()
    {
        return Enum.IsDefined(typeof(BlogStatus), Status);
    }
}