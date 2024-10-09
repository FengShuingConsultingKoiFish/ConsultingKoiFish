using ConsultingKoiFish.BLL.DTOs.AdvertisementPackageDTOs;
using ConsultingKoiFish.BLL.DTOs.PackageImageDTOs;
using ConsultingKoiFish.BLL.DTOs.Response;

namespace ConsultingKoiFish.BLL.Services.Interfaces;

public interface IAdvertisementPackageService
{
    Task<BaseResponse> CreateUpdateAdvertisementPackage(AdvertisementPackageRequestDTO dto, string userName);
    Task<BaseResponse> AddImagesToPackage(PackageImageRequestDTO dto);
    Task<BaseResponse> DeleteImagesFromoPackage(PackageImageDeleteDTO dto);
}