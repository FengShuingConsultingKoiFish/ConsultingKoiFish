using ConsultingKoiFish.BLL.DTOs.Response;
using ConsultingKoiFish.BLL.DTOs.UserDetailDTOs;
using ConsultingKoiFish.DAL.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.BLL.Services.Interfaces
{
	public interface IUserDetailService
	{
		Task<BaseResponse> CreateUpdateUserDetail(UserDetailRequestDTO dto, string userId);
		Task<BaseResponse> UpdateAvatar(UserDetailUpdateAvatarDTO dto, string userId);
		Task<BaseResponse> DeleteAvatar(string userId);
		Task<UserDetailViewDTO> GetUserDetailByUserId(string userId);
		Task<string> GetUserAvatarByUserId(string userId);
		Task<string> GetUserAvatarByUserName(string userName);
		Task<UserDetailViewDTO> GetUserDetailById(Guid id);
		Task<UserDetailViewDTO> GetUserDetailByUserName(string userName);
		Task<PaginatedList<UserDetailViewDTO>> GetAllUserDetails(UserDetailGetListDTO dto);
		Task<BaseResponse> DeleteUserDetail(string userId);
	}
}
