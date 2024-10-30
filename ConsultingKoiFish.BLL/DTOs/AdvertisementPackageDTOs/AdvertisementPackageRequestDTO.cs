using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ConsultingKoiFish.BLL.DTOs.AdvertisementPackageDTOs;

public class AdvertisementPackageRequestDTO
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Tên gói không được để trống.")]
    public string Name { get; set; } = null!;
    [Required(ErrorMessage = "Giá gói không được để trống.")]
    public double Price { get; set; }
    public string? Description { get; set; }
    public int? LimitAd { get; set; }
    public int? LimitContent { get; set; }
    public int? LimitImage { get; set; }
    [Required(ErrorMessage = "Thời hạn không được để trống.")]
    public int DurationsInDays { get; set; }
    public List<int>? ImageIds { get; set; }
}