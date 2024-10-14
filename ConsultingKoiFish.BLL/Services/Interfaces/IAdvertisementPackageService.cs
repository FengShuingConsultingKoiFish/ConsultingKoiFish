using ConsultingKoiFish.BLL.DTOs.AdvertisementPackageDTOs;
using ConsultingKoiFish.BLL.DTOs.PackageImageDTOs;
using ConsultingKoiFish.BLL.DTOs.Response;
using ConsultingKoiFish.BLL.Helpers.Fillters;
using ConsultingKoiFish.DAL.Enums;
using ConsultingKoiFish.DAL.Paging;

namespace ConsultingKoiFish.BLL.Services.Interfaces;

public interface IAdvertisementPackageService
{
    Task<BaseResponse> CreateUpdateAdvertisementPackage(AdvertisementPackageRequestDTO dto, string userName);
    Task<BaseResponse> AddImagesToPackage(PackageImageRequestDTO dto);
    Task<BaseResponse> DeleteImagesFromPackage(PackageImageDeleteDTO dto);
    Task<BaseResponse> DeletePackage(int id, string userName);
    Task<AdvertisementPackageViewDTO> GetPackageById(int id, OrderImage? orderImage);
    Task<PaginatedList<AdvertisementPackageViewDTO>> GetAllPackages(PackageGetListDTO dto);
}