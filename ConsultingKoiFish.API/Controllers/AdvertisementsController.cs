using ConsultingKoiFish.BLL.DTOs;
using ConsultingKoiFish.BLL.DTOs.AdImageDTOs;
using ConsultingKoiFish.BLL.DTOs.AdvertisementDTOs;
using ConsultingKoiFish.BLL.DTOs.BlogDTOs;
using ConsultingKoiFish.BLL.DTOs.BlogImageDTOs;
using ConsultingKoiFish.BLL.Services.Implements;
using ConsultingKoiFish.BLL.Services.Interfaces;
using ConsultingKoiFish.DAL.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ConsultingKoiFish.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AdvertisementsController : BaseAPIController
	{
		private readonly IAdvertisementService _advertisementService;

		public AdvertisementsController(IAdvertisementService advertisementService)
        {
			this._advertisementService = advertisementService;
		}


		#region Common

		/// <summary>
		/// This is used to get all ads in website
		/// </summary>
		/// <param name="dto"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("get-all-advertisements")]
		public async Task<IActionResult> GetAllAdvertisements(AdvertisementGetListDTO dto)
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

				if (!dto.IsValidAdvertisementStatus())
				{
					ModelState.AddModelError("AdvertisementStatus", "Status không hợp lệ.");
					return ModelInvalid();
				}

				if (!dto.IsValidOrderAdvertisement())
				{
					ModelState.AddModelError("OrderAdvertisement", "OrderBlog không hợp lệ.");
					return ModelInvalid();
				}

				if (!dto.IsValidOrderImage())
				{
					ModelState.AddModelError("OrderImage", "OrderImage không hợp lệ.");
					return ModelInvalid();
				}

				if (!dto.IsValidOrderComment())
				{
					ModelState.AddModelError("OrderComment", "OrderComment không hợp lệ.");
					return ModelInvalid();
				}

				var data = await _advertisementService.GetAllAdvertisements(dto);
				var response = new PagingDTO<AdvertisementViewDTO>(data);
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

		/// <summary>
		/// This is used to get an ad by its id
		/// </summary>
		/// <param name="id"></param>
		/// <param name="orderComment"></param>
		/// <param name="orderImage"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("get-advertisement-by-id/{id}/{orderComment}/{orderImage}")]
		public async Task<IActionResult> GetAdvertisementById([FromRoute] int id, [FromRoute] OrderComment? orderComment, [FromRoute] OrderImage? orderImage)
		{
			try
			{
				var response = await _advertisementService.GetAdvertisementById(id, orderComment, orderImage);
				if (response == null) return GetError("Quảng cáo này không tồn tại.");
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

		#endregion

		#region Member

		/// <summary>
		/// this is used to create and update advertisement
		/// </summary>
		/// <param name="dto"></param>
		/// <returns></returns>
		[Authorize(Roles = "Member")]
		[HttpPost]
		[Route("create-update-advertisement")]
		public async Task<IActionResult> CreateUpdateAdvertisement(AdvertisementRequestDTO dto)
		{
			try
			{
				if (!ModelState.IsValid) return ModelInvalid();
				var response = await _advertisementService.CreateUpdateAdvertisement(dto, UserId);
				if (!response.IsSuccess) return SaveError(response);
				return SaveSuccess(response);
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ResetColor();
				return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút nữa.");
			}
		}

		/// <summary>
		/// this is used to add images to current advertisement
		/// </summary>
		/// <param name="dto"></param>
		/// <returns></returns>
		[Authorize(Roles = "Member")]
		[HttpPost]
		[Route("add-images-to-advertisement")]
		public async Task<IActionResult> AddImagesToAdvertisements(AdImageRequestDTO dto)
		{
			try
			{
				if (!ModelState.IsValid) return ModelInvalid();
				var response = await _advertisementService.AddImagesToAdvertisement(dto);
				if (!response.IsSuccess) return SaveError(response);
				return SaveSuccess(response);
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ResetColor();
				return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút nữa.");
			}
		}

		/// <summary>
		/// This is used to delete images from a advertisement
		/// </summary>
		/// <param name="dto"></param>
		/// <returns></returns>
		[Authorize(Roles = "Member")]
		[HttpPost]
		[Route("delete-images-from-advertisement")]
		public async Task<IActionResult> DeleteImagesFromAdvertisement(AdImageDeleteDTO dto)
		{
			try
			{
				if (!ModelState.IsValid) return ModelInvalid();
				var response = await _advertisementService.DeleteImagesFromAdvertisement(dto);
				if (!response.IsSuccess) return SaveError(response);
				return SaveSuccess(response);
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
		[HttpPost]
		[Route("get-all-advertisements-for-user")]
		public async Task<IActionResult> GetAllAdvertisementsForUser(AdvertisementGetListDTO dto)
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

				if (!dto.IsValidAdvertisementStatus())
				{
					ModelState.AddModelError("AdvertisementStatus", "Status không hợp lệ.");
					return ModelInvalid();
				}

				if (!dto.IsValidOrderAdvertisement())
				{
					ModelState.AddModelError("OrderAdvertisement", "OrderBlog không hợp lệ.");
					return ModelInvalid();
				}

				if (!dto.IsValidOrderImage())
				{
					ModelState.AddModelError("OrderImage", "OrderImage không hợp lệ.");
					return ModelInvalid();
				}

				if (!dto.IsValidOrderComment())
				{
					ModelState.AddModelError("OrderComment", "OrderComment không hợp lệ.");
					return ModelInvalid();
				}

				var data = await _advertisementService.GetAllAdvertisementsForUser(UserId, dto);
				var response = new PagingDTO<AdvertisementViewDTO>(data);
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

		#endregion

		#region Admin

		[HttpPost]
		[Route("get-all-advertisements-for-admin")]
		public async Task<IActionResult> GetAllAdvertisementsForAdmin(AdvertisementGetListDTO dto)
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

				if (!dto.IsValidAdvertisementStatus())
				{
					ModelState.AddModelError("AdvertisementStatus", "Status không hợp lệ.");
					return ModelInvalid();
				}

				if (!dto.IsValidOrderAdvertisement())
				{
					ModelState.AddModelError("OrderAdvertisement", "OrderBlog không hợp lệ.");
					return ModelInvalid();
				}

				if (!dto.IsValidOrderImage())
				{
					ModelState.AddModelError("OrderImage", "OrderImage không hợp lệ.");
					return ModelInvalid();
				}

				if (!dto.IsValidOrderComment())
				{
					ModelState.AddModelError("OrderComment", "OrderComment không hợp lệ.");
					return ModelInvalid();
				}

				var data = await _advertisementService.GetAllBlogsForAdmin(dto);
				var response = new PagingDTO<AdvertisementViewDTO>(data);
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

		#endregion
	}
}
