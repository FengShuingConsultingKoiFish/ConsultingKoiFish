using ConsultingKoiFish.BLL.DTOs.ImageDTOs;
using ConsultingKoiFish.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ConsultingKoiFish.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ImagesController : BaseAPIController
	{
		private readonly IImageService _imageService;

		public ImagesController(IImageService imageService)
        {
			this._imageService = imageService;
		}

		[Authorize(Roles = "Member")]
		[HttpPost]
		[Route("create-update-image")]
		public async Task<IActionResult> CreateUpdateImage(ImageRequestDTO dto)
		{
			try
			{
				if(dto.Id == 0)
				{
					ModelState.AddModelError("Id", "Id phải là số nguyên lớn hơn 0.");
					return ModelInvalid();
				}

				var response = await _imageService.CreateUpdateImage(dto, UserId);
				if(!response.IsSuccess) return SaveError(response);
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
    }
}
