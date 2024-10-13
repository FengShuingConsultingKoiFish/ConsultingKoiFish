using ConsultingKoiFish.BLL.DTOs.PurchasedPackageDTOs;
using ConsultingKoiFish.BLL.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.BLL.Services.Interfaces
{
	public interface IPurchasedPackageService
	{
		Task<BaseResponse> CreatePurchasedPacakge(PurchasedPackageCreateDTO dto);
	}
}
