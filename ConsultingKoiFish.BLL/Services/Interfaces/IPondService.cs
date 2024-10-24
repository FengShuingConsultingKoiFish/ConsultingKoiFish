using ConsultingKoiFish.BLL.DTOs.PondDTOs;
using ConsultingKoiFish.BLL.DTOs.Response;

namespace ConsultingKoiFish.BLL.Services.Interfaces;

public interface IPondService
{
    Task<BaseResponse> AddPondCategory(PondCategoryDTO pondCategoryDto);
    Task<BaseResponse> UpdatePondCategory(PondCategoryDTO pondCategoryDto, int pondCategoryId);
    Task<ResponseApiDTO> GetAllPondCategory();
    Task<BaseResponse> DeletePondCategory(int pondCategoryId);
    Task<BaseResponse> AddPondCharacteristic(PondCharacteristicDTO pondCharacteristicDto);
    Task<BaseResponse> UpdatePondCharacteristic(PondCharacteristicDTO pondCharacteristicDto, int pondCharacteristicId);
    Task<ResponseApiDTO> GetAllPondCharacteristic();
    Task<BaseResponse> DeletePondCharacteristic(int pondCharacteristicId);
    Task<BaseResponse> AddSuitablePondZodiac(ZodiacPondDTO zodiacPondDto);
    Task<BaseResponse> UpdatePondZodiac(ZodiacPondDTO zodiacPondDto, int zodiacPondId);
    Task<ResponseApiDTO> GetAllPondZodiac();
    Task<BaseResponse> DeletePondZodiac(int zodiacPondId);
    Task<ResponseApiDTO> GetSuitablePondForUser(string UserId);
    Task<ResponseApiDTO> GetPondByPondCategory(int pondCategoryId);
}