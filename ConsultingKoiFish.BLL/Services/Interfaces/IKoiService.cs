using ConsultingKoiFish.BLL.DTOs.KoiDTOs;
using ConsultingKoiFish.BLL.DTOs.Response;


namespace ConsultingKoiFish.BLL.Services.Interfaces;

public interface IKoiService
{
    Task<BaseResponse> AddKoiCategory(KoiCategoryDTO koiCategoryDto);
    Task<BaseResponse> UpdateKoiCategory(KoiCategoryDTO koiCategoryDto, int koiCategoryId);
    Task<ResponseApiDTO> GetAllKoiCategory();
    Task<BaseResponse> DeleteKoiCategory(int koiCategoryId);
    Task<BaseResponse> AddKoiBreed(KoiBreedDTO koiBreedDto);
    Task<BaseResponse> UpdateKoiBreed(KoiBreedDTO koiBreedDto, int koiBreedId);
    Task<ResponseApiDTO> GetAllKoiBreed();
    Task<BaseResponse> DeleteKoiBreed(int koiBreedId);
    Task<BaseResponse> AddSuitableKoiZodiac(ZodiacKoiBreedDTO zodiacKoiBreedDto);
    Task<BaseResponse> UpdateKoiZodiac(ZodiacKoiBreedDTO zodiacKoiBreedDto, int koiZodiacId);
    Task<ResponseApiDTO> GetAllKoiZodiac();
    Task<BaseResponse> DeleteKoiZodiac(int koiZodiacId);
    Task<ResponseApiDTO> GetSuitableKoiForUser(string UserId);
    Task<ResponseApiDTO> GetKoiBreedByKoiCategory(int koiCategoryId);
}