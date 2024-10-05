﻿using ConsultingKoiFish.BLL.DTOs;
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

		[Authorize()]
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

		[Authorize()]
		[HttpGet]
		[Route("get-image-by-id/{id}")]
		public async Task<IActionResult> CreateUpdateImage([FromRoute] int id)
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

		[Authorize()]
		[HttpGet]
		[Route("get-images-by-userid/{pageIndex}/{pageSize}")]
		public async Task<IActionResult> GetListImageByUserId([FromRoute] int pageIndex, [FromRoute] int pageSize)
		{
			try
			{
				if (pageIndex <= 0)
				{
					return GetError("Page Index phải là số nguyên dương.");
				}

				if (pageSize <= 0)
				{
					return GetError("Page Size phải là số nguyên dương.");
				}

				var data = await _imageService.GetListImageByUserId(UserId, pageIndex, pageSize);
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

		[Authorize()]
		[HttpGet]
		[Route("get-images-by-alt-text/{pageIndex}/{pageSize}")]
		public async Task<IActionResult> GetListImageByUserId(string? name, [FromRoute] int pageIndex, [FromRoute] int pageSize)
		{
			try
			{
				if (pageIndex <= 0)
				{
					return GetError("Page Index phải là số nguyên dương.");
				}

				if (pageSize <= 0)
				{
					return GetError("Page Size phải là số nguyên dương.");
				}

				var data = await _imageService.GetListImageByName(name, UserId, pageIndex, pageSize);
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

		[Authorize()]
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