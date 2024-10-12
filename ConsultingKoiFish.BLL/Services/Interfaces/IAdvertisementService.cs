using ConsultingKoiFish.BLL.DTOs.AdvertisementDTOs;
using ConsultingKoiFish.BLL.DTOs.Response;
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
	}
}
