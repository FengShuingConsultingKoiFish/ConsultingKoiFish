using ConsultingKoiFish.BLL.DTOs.ImageDTOs;
using ConsultingKoiFish.BLL.DTOs.Response;
using ConsultingKoiFish.DAL.Entities;
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
		Task<ImageViewDTO> CreateImage(ImageRequestDTO requestDTO, string userId);
		Task<ImageViewDTO> GetImageById(int id);
		Task<PaginatedList<ImageViewDTO>> GetListImageForMember(ImageGetListDTO dto, string userId);
		Task<BaseResponse> DeleteImage(int id, string userId);
		Task<List<ImageViewDTO>> ConvertSpeciedImageToImageViews<TImage>(ICollection<TImage> images, Func<TImage, int> getImageId);
		List<ImageViewDTO> ConvertToImageViews(ICollection<Image> images);
	}
}
