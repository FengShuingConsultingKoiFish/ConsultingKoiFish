using ConsultingKoiFish.BLL.DTOs.Response;
using ConsultingKoiFish.BLL.DTOs.ZodiacDTO;

namespace ConsultingKoiFish.BLL.Services.Interfaces;

public interface IZodiacService
{
    Task<BaseResponse> AddZodiac(ZodiacRequestDTO zodiacRequestDto);
    Task<BaseResponse> UpdateZodiac(ZodiacRequestDTO zodiacRequestDto, int zodiacID);
    Task<ResponseApiDTO> GetAllZodiacs();
    Task<BaseResponse> DeleteZodiac(int zodiacID);
    Task<ResponseApiDTO> GetZodiacByBirthDate(string userId);
    Task<ResponseApiDTO> GetZodiacByBirthDate(string name, DateTime birthDate);
}