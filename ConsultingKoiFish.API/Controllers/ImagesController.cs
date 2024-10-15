using ConsultingKoiFish.BLL.DTOs;
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

		[Authorize]
		[HttpPost]
		[Route("create-update-image")]
		public async Task<IActionResult> CreateUpdateImage(ImageRequestDTO dto)
		{
			try
			{
				var response = await _imageService.CreateUpdateImage(dto, UserId);
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
		
		[HttpGet]
		[Route("get-image-by-id/{id}")]
		public async Task<IActionResult> GetImageById([FromRoute] int id)
		{
			try
			{
				var response = await _imageService.GetImageById(id);
				if (response == null) return GetError("Ảnh này không tồn tại.");
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

		[Authorize]
		[HttpPost]
		[Route("get-images-for-member")]
		public async Task<IActionResult> GetListImageForMember(ImageGetListDTO dto)
		{
			try
			{
				if (!ModelState.IsValid) return ModelInvalid();
				if (dto.PageIndex <= 0)
				{
					ModelState.AddModelError("PageIndex", "Page Index phải là số nguyên dương.");
					return ModelInvalid();
				}

				if (dto.PageSize <= 0)
				{
					ModelState.AddModelError("PageSize", "Page Size phải là số nguyên dương.");
					return ModelInvalid();
				}

				if(!dto.IsValidOrderDate())
				{
					ModelState.AddModelError("OrderDate", "OrderDate không hợp lệ.");
					return ModelInvalid();
				} 

				var data = await _imageService.GetListImageForMember(dto, UserId);
				var response = new PagingDTO<ImageViewDTO>(data);
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

		[Authorize]
		[HttpPost]
		[Route("delete-image/{id}")]
		public async Task<IActionResult> DeleteImage(int id)
		{
			try
			{
				var response = await _imageService.DeleteImage(id, UserId);
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
	}
}
