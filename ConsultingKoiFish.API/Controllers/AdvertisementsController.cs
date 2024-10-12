using ConsultingKoiFish.BLL.DTOs.AdImageDTOs;
using ConsultingKoiFish.BLL.DTOs.AdvertisementDTOs;
using ConsultingKoiFish.BLL.DTOs.BlogDTOs;
using ConsultingKoiFish.BLL.DTOs.BlogImageDTOs;
using ConsultingKoiFish.BLL.Services.Implements;
using ConsultingKoiFish.BLL.Services.Interfaces;
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

		#endregion
	}
}
