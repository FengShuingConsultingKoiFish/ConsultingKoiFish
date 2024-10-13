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
		Task<UserDetailViewDTO> GetUserDetailByUserId(string userId);
		Task<PaginatedList<UserDetailViewDTO>> GetAllUserDetails(int pageIndex, int pageSize);
		Task<PaginatedList<UserDetailViewDTO>> GetAllUserDetailsByName(int pageIndex, int pageSize, string? name);
		Task<BaseResponse> DeleteUserDetail(string userId);
	}
}
