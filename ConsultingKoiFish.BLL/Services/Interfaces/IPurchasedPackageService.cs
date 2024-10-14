using ConsultingKoiFish.BLL.DTOs.PurchasedPackageDTOs;
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
	public interface IPurchasedPackageService
	{
		Task<BaseResponse> CreatePurchasedPacakge(PurchasedPackageCreateDTO dto);
		Task<PaginatedList<PurchasedPackageViewDTO>> GetAllPurchasedPackageForUser(PurchasedPackageGetListDTO dto, string userId);
		Task<PurchasedPackageViewDTO> GetPurchasedPackageByIdForMember(int id, string userId, OrderImage? orderImage);
		Task<PaginatedList<PurchasedPackageViewDTO>> GetAllPurchasedPackageForAdmin(PurchasedPackageGetListDTO dto);
		Task<PurchasedPackageViewDTO> GetPurchasedPackageByIdForAdmin(int id, OrderImage? orderImage);
	}
}
