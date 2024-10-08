using System.ComponentModel.DataAnnotations;

namespace ConsultingKoiFish.BLL.DTOs.PackageImageDTOs;

public class PackageImageCreateDTO
{
    [Required(ErrorMessage = "AdvertisementPackageId không được để trống.")]
    public int AdvertisementPackageId { get; set; }
    [Required(ErrorMessage = "ImageId không được để trống.")]
    public int ImageId { get; set; }
}