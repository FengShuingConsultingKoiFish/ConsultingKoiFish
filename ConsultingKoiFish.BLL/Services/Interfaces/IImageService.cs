using ConsultingKoiFish.BLL.DTOs.ImageDTOs;
using ConsultingKoiFish.BLL.DTOs.Response;
using ConsultingKoiFish.DAL.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.BLL.Services.Interfaces
{
	public interface IImageService
	{
		Task<BaseResponse> CreateUpdateImage(ImageRequestDTO requestDTO, string userId);
		Task<ImageViewDTO> GetImageById(int id);
		Task<PaginatedList<ImageViewDTO>> GetListImageByUserId(string userId, int pageIndex, int pageSize);
		Task<PaginatedList<ImageViewDTO>> GetListImageByName(string? name, string userId, int pageIndex, int pageSize);
		Task<BaseResponse> DeleteImage(int id, string userId);
	}
}
