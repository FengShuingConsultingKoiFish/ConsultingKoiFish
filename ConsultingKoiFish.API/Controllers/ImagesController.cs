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
		private readonly IConfiguration _configuration;
		private readonly long _fileSizeLimit = 10 * 1024 * 1024;

		public ImagesController(IImageService imageService, IConfiguration configuration)
		{
			this._imageService = imageService;
			this._configuration = configuration;
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

		[Authorize]
		[HttpPost]
		[Route("upload-image")]
		public async Task<IActionResult> UploadImage(ImageUploadDTO dto)
		{
			try
			{
				using (var memoryStream = new MemoryStream())
				{
					await dto.File.CopyToAsync(memoryStream);
					var extension = Path.GetExtension(dto.File.FileName);
					var isSvg = extension.Equals(".svg");
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
					var guidFileName = Path.GetRandomFileName();
					var guildStringPath = new string[] { "images", IsAdmin ? String.Empty : "stock-photo", $"{guidFileName}{extension}" };
					var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", Helpers.PathCombine(guildStringPath));
					string directory = Path.GetDirectoryName(path);
					if (!Directory.Exists(directory))
						Directory.CreateDirectory(directory);
					using (var fileStream = System.IO.File.Create(path))
					{
						await fileStream.WriteAsync(memoryStream.ToArray());
						fileStream.Close();
					}


					string cdnhost = _configuration.GetSection("AppSettings").GetValue<string>("CdnUrl");
					string imageUrl = $"{cdnhost}{Helpers.UrlCombine(guildStringPath)}";
					string thumbUrl = isSvg ? imageUrl : $"{cdnhost}/{CompressThumbnailWithNew(path)}";
					return SaveSuccess(new
					{
						Success = true,
						ImageUrl = imageUrl,
						ThumbnailUrl = thumbUrl
					});
				}
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
