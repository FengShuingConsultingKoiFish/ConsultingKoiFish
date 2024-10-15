using ConsultingKoiFish.BLL.DTOs;
using ConsultingKoiFish.BLL.DTOs.Response;

namespace ConsultingKoiFish.BLL.Services.Interfaces;

public interface IUserPondService
{
    Task<BaseResponse> AddUserPond(UserPondDTOs userPondDtOs);
    Task<BaseResponse> UpdateUserPond(UserPondDTOs userPondDtOs, int userPondId);
    Task<ResponseApiDTO> GetAllUserPond(string userId);
    Task<BaseResponse> DeleteUserPond(int userPondId);

    public Task<BaseResponse> AddKoiAndPondDetails(
        KoiAndPondDetailRequestDTOs.KoiAndPondDetailRequestDTO requestDto);

    public Task<BaseResponse> UpdateKoiAndPondDetails(int userPondId,
        KoiAndPondDetailRequestDTOs.KoiAndPondDetailRequestDTO requestDto);

    public Task<ResponseApiDTO> ViewKoiAndPondDetails(int userPondId);
}