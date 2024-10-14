using ConsultingKoiFish.BLL.DTOs.BlogDTOs;
using ConsultingKoiFish.BLL.DTOs;
using ConsultingKoiFish.BLL.Services.Implements;
using ConsultingKoiFish.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ConsultingKoiFish.BLL.DTOs.PurchasedPackageDTOs;
using ConsultingKoiFish.DAL.Enums;

namespace ConsultingKoiFish.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PurchasedPackagesController : BaseAPIController
	{
		private readonly IPurchasedPackageService _purchasedPackageService;

		public PurchasedPackagesController(IPurchasedPackageService purchasedPackageService)
        {
			this._purchasedPackageService = purchasedPackageService;
		}

		#region Member


		[Authorize(Roles = "Member")]
		[HttpPost]
		[Route("get-all-purchased-package-for-user")]
		public async Task<IActionResult> GetAllPurchasedPackagesByUserId(PurchasedPackageGetListDTO dto)
		{
			try
			{
				if (!ModelState.IsValid) return ModelInvalid();
				if (dto.PageIndex <= 0)
				{
					ModelState.AddModelError("PageIndex", "PageIndex phải là số nguyên dương");
					return ModelInvalid();
				}

				if (dto.PageSize <= 0)
				{
					ModelState.AddModelError("PageSize", "PageIndex phải là số nguyên dương");
					return ModelInvalid();
				}

				if (!dto.IsValidOrderImage())
				{
					ModelState.AddModelError("OrderImage", "OrderImage không hợp lệ.");
					return ModelInvalid();
				}

				if (!dto.IsValidStatus())
				{
					ModelState.AddModelError("Status", "Status không hợp lệ.");
					return ModelInvalid();
				}

				if (!dto.IsValidOrderDate())
				{
					ModelState.AddModelError("OrderDate", "OrderDate không hợp lệ.");
					return ModelInvalid();
				}


				var data = await _purchasedPackageService.GetAllPurchasedPackageForUser(dto, UserId);
				var response = new PagingDTO<PurchasedPackageViewDTO>(data);
				if (response == null) return GetError();
				return GetSuccess(response);
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ResetColor();
				return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút nữa.");
			}
		}


		[Authorize(Roles = "Member")]
		[HttpGet]
		[Route("get-purchased-package-by-id-for-user/{id}")]
		public async Task<IActionResult> GetPurchasedPackageByUserIdForMember([FromRoute]int id, OrderImage? orderImage)
		{
			try
			{
				var data = await _purchasedPackageService.GetPurchasedPackageByIdForMember(id, UserId, orderImage);
				if (data == null) return GetError();
				return GetSuccess(data);
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ResetColor();
				return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút nữa.");
			}
		}

		#endregion


		#region Admin

		[Authorize(Roles = "Admin")]
		[HttpPost]
		[Route("get-all-purchased-package-for-admin")]
		public async Task<IActionResult> GetAllPurchasedPackagesForAdmin(PurchasedPackageGetListDTO dto)
		{
			try
			{
				if (!ModelState.IsValid) return ModelInvalid();
				if (dto.PageIndex <= 0)
				{
					ModelState.AddModelError("PageIndex", "PageIndex phải là số nguyên dương");
					return ModelInvalid();
				}

				if (dto.PageSize <= 0)
				{
					ModelState.AddModelError("PageSize", "PageIndex phải là số nguyên dương");
					return ModelInvalid();
				}

				if (!dto.IsValidOrderImage())
				{
					ModelState.AddModelError("OrderImage", "OrderImage không hợp lệ.");
					return ModelInvalid();
				}

				if (!dto.IsValidStatus())
				{
					ModelState.AddModelError("Status", "Status không hợp lệ.");
					return ModelInvalid();
				}

				if (!dto.IsValidOrderDate())
				{
					ModelState.AddModelError("OrderDate", "OrderDate không hợp lệ.");
					return ModelInvalid();
				}


				var data = await _purchasedPackageService.GetAllPurchasedPackageForAdmin(dto);
				var response = new PagingDTO<PurchasedPackageViewDTO>(data);
				if (response == null) return GetError();
				return GetSuccess(response);
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ResetColor();
				return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút nữa.");
			}
		}


		[Authorize(Roles = "Admin")]
		[HttpGet]
		[Route("get-purchased-package-by-id-for-admin/{id}")]
		public async Task<IActionResult> GetPurchasedPackageByUserIdForAdmin([FromRoute] int id, OrderImage? orderImage)
		{
			try
			{
				var data = await _purchasedPackageService.GetPurchasedPackageByIdForAdmin(id, orderImage);
				if (data == null) return GetError();
				return GetSuccess(data);
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ResetColor();
				return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút nữa.");
			}
		}

		#endregion
	}
}
