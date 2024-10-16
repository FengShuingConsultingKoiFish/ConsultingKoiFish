using System.Net;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using ConsultingKoiFish.BLL.DTOs;
using ConsultingKoiFish.BLL.DTOs.ImageDTOs;
using ConsultingKoiFish.BLL.Helpers.Config;
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
		private readonly IConfiguration _configuration;
		private readonly Cloudinary _cloudinary;
		private readonly long _fileSizeLimit = 10 * 1024 * 1024;

		public ImagesController(IImageService imageService, IConfiguration configuration, Cloudinary cloudinary)
		{
			this._imageService = imageService;
			this._configuration = configuration;
			_cloudinary = cloudinary;
		}

		
		[Authorize]
		[HttpPost]
		[Route("upload-image")]
		public async Task<IActionResult> UploadImage(ImageUploadDTO dto)
		{
			try
			{
				if (!ModelState.IsValid) return ModelInvalid();
				dto.ProcessFileName(isCamelCase: true);
				var isValid = IsValidFileExtension(dto.File.FileName, new string[] { ".jpg", ".png", ".svg", ".jpeg", ".dng" });
				if (!isValid)
				{
					ModelState.AddModelError("File", "Không hỗ trợ định dạng ảnh hiện tại.");
					return ModelInvalid();
				}
				
				if (dto.File.Length > _fileSizeLimit)
				{
					var megabyteSizeLimit = _fileSizeLimit / 1048576;
					ModelState.AddModelError("File", $"Kích thước ảnh vượt quá quy định cho phép ({megabyteSizeLimit:N1} MB).");
					return ModelInvalid();
				}
				
				var uploadParams = new ImageUploadParams
				{
					File = new FileDescription(dto.File.FileName, dto.File.OpenReadStream())
				};
				
				var uploadResult = await _cloudinary.UploadAsync(uploadParams);
				if (uploadResult.Error != null)
				{
					Console.WriteLine($"Cloudinary error: {uploadResult.Error.Message}");
					return StatusCode(500, $"Upload failed: {uploadResult.Error.Message}");
				}

				if (uploadResult.StatusCode != HttpStatusCode.OK)
					return Error("Upload ảnh không thành công.");

				var imageRequestDTO = new ImageRequestDTO
				{
					Id = 0,
					Url = uploadResult.SecureUrl.ToString(),
					AltText = dto.FileName
				};
				
				var addImageToDB = await _imageService.CreateImage(imageRequestDTO, UserId);
				if (addImageToDB == null) return SaveError(addImageToDB);
				return SaveSuccess(await _imageService.GetImageById(addImageToDB.Id));
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.WriteLine(ex.StackTrace);
				Console.ResetColor();
				return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút nữa.");
			}
		}
		
		[Authorize]
		[HttpPost]
		[Route("create-update-image")]
		public async Task<IActionResult> CreateUpdateImage(ImageRequestDTO dto)
		{
			try
			{
				if (!ModelState.IsValid) return ModelInvalid();
				dto.ProcessFileName(isCamelCase: true);
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


		#region PRIVATE

		private bool IsValidFileExtension(string fileName, string[] allowedExtensions)
		{
			var extension = Path.GetExtension(fileName).ToLowerInvariant();
			return allowedExtensions.Contains(extension);
		}

		private string CompressThumbnailWithNew(string imagePath)
		{

			var thumbnailFileName = Path.GetFileNameWithoutExtension(imagePath) + "_thumb" + Path.GetExtension(imagePath);
			var thumbnailPath = Path.Combine(Path.GetDirectoryName(imagePath), thumbnailFileName);


			return thumbnailPath;
		}

		#endregion
	}
}
