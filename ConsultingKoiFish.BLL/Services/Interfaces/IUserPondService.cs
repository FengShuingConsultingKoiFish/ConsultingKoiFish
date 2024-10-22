using ConsultingKoiFish.BLL.DTOs;
using ConsultingKoiFish.BLL.DTOs.Response;

namespace ConsultingKoiFish.BLL.Services.Interfaces;

public interface IUserPondService
{
    Task<BaseResponse> AddUserPond(UserPondDTOs userPondDtOs, string userId);
    Task<BaseResponse> UpdateUserPond(UserPondDTOs userPondDtOs, int userPondId);
    Task<ResponseApiDTO> GetAllUserPond(string userId);
    Task<BaseResponse> DeleteUserPond(int userPondId);

    public Task<ResponseApiDTO> AddKoiAndPondDetails(
        KoiAndPondDetailRequestDTOs.KoiAndPondDetailRequestDTO requestDto);

    public Task<BaseResponse> UpdateKoiAndPondDetails(int userPondId,
        KoiAndPondDetailRequestDTOs.KoiAndPondDetailRequestDTO requestDto);

    public Task<ResponseApiDTO> ViewKoiAndPondDetails(int userPondId);
}