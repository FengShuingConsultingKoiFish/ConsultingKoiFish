using ConsultingKoiFish.BLL.DTOs.AdImageDTOs;
using ConsultingKoiFish.BLL.DTOs.AdvertisementDTOs;
using ConsultingKoiFish.BLL.DTOs.BlogDTOs;
using ConsultingKoiFish.BLL.DTOs.BlogImageDTOs;
using ConsultingKoiFish.BLL.DTOs.Response;
using ConsultingKoiFish.DAL.Enums;
using ConsultingKoiFish.DAL.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.BLL.Services.Interfaces
{
	public interface IAdvertisementService
	{
		Task<BaseResponse> CreateUpdateAdvertisement(AdvertisementRequestDTO dto, string userId);
		Task<BaseResponse> AddImagesToAdvertisement(AdImageRequestDTO dto);
		Task<BaseResponse> DeleteImagesFromAdvertisement(AdImageDeleteDTO dto);
		Task<PaginatedList<AdvertisementViewDTO>> GetAllAdvertisements(AdvertisementGetListDTO dto);
		Task<AdvertisementViewDTO> GetAdvertisementById(int id, OrderComment? orderComment, OrderImage? orderImage);
		Task<PaginatedList<AdvertisementViewDTO>> GetAllAdvertisementsForUser(string userId, AdvertisementGetListDTO dto);
		Task<PaginatedList<AdvertisementViewDTO>> GetAllBlogsForAdmin(AdvertisementGetListDTO dto);
		Task<BaseResponse> UpdateStatusAdvertisement(AdvertisementUpdateStatusDTO dto);
	}
}
