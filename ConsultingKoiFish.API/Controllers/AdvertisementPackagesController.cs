using ConsultingKoiFish.BLL.DTOs;
using ConsultingKoiFish.BLL.DTOs.AdvertisementPackageDTOs;
using ConsultingKoiFish.BLL.DTOs.BlogDTOs;
using ConsultingKoiFish.BLL.DTOs.BlogImageDTOs;
using ConsultingKoiFish.BLL.DTOs.PackageImageDTOs;
using ConsultingKoiFish.BLL.DTOs.PaymentDTOs;
using ConsultingKoiFish.BLL.DTOs.VnPayDTOs;
using ConsultingKoiFish.BLL.Helpers.Config;
using ConsultingKoiFish.BLL.Helpers.Fillters;
using ConsultingKoiFish.BLL.Services.Implements;
using ConsultingKoiFish.BLL.Services.Interfaces;
using ConsultingKoiFish.DAL.Entities;
using ConsultingKoiFish.DAL.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ConsultingKoiFish.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdvertisementPackagesController : BaseAPIController
    {
        private readonly IAdvertisementPackageService _advertisementPackageService;
		private readonly IUserDetailService _userDetailService;
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IConfiguration _configuration;

		public AdvertisementPackagesController(IAdvertisementPackageService advertisementPackageService,
												IUserDetailService userDetailService,
												IHttpClientFactory httpClientFactory,
												IConfiguration configuration)
        {
            _advertisementPackageService = advertisementPackageService;
			this._userDetailService = userDetailService;
			this._httpClientFactory = httpClientFactory;
			this._configuration = configuration;
		}
        
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("create-update-advertisement-package")]
        public async Task<IActionResult> CreateUpdateAdvertisementPackage(AdvertisementPackageRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid) return ModelInvalid();
                if(dto.ImageIds.Contains(0))
                {
                    ModelState.AddModelError("ImageIds", "CommentId ảnh phải là số nguyên dương.");
                    return ModelInvalid();
                }
                var response = await _advertisementPackageService.CreateUpdateAdvertisementPackage(dto, UserName);
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

		[Authorize(Roles = "Admin")]
		[HttpPost]
		[Route("add-images-to-packages")]
		public async Task<IActionResult> AddImagesToPackage(PackageImageRequestDTO dto)
		{
			try
			{
				if (!ModelState.IsValid) return ModelInvalid();
				var response = await _advertisementPackageService.AddImagesToPackage(dto);
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

		[Authorize(Roles = "Admin")]
		[HttpPost]
		[Route("delete-images-from-package")]
		public async Task<IActionResult> DeleteImagesFromBlog(PackageImageDeleteDTO dto)
		{
			try
			{
				if (!ModelState.IsValid) return ModelInvalid();
				var response = await _advertisementPackageService.DeleteImagesFromPackage(dto);
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

		[Authorize(Roles = "Admin")]
		[HttpPost]
		[Route("delete-package/{id}")]
		public async Task<IActionResult> DeletePackage(int id)
		{
			try
			{
				var response = await _advertisementPackageService.DeletePackage(id, UserName);
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

		#region Common

		[HttpGet]
		[Route("get-package-by-id/{id}")]
		public async Task<IActionResult> GetPackageById([FromRoute] int id, OrderImage? orderImage)
		{
			try
			{
				var response = await _advertisementPackageService.GetPackageById(id, orderImage);
				if (response == null) return GetError("Gói này không tồn tại.");
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

		[HttpPost]
		[Route("get-all-packages")]
		public async Task<IActionResult> GetAllPackages(PackageGetListDTO dto)
		{
			try
			{
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

				var data = await _advertisementPackageService.GetAllPackages(dto);
				var response = new PagingDTO<AdvertisementPackageViewDTO>(data);
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
