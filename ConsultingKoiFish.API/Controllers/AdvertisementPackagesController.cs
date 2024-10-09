using ConsultingKoiFish.BLL.DTOs.AdvertisementPackageDTOs;
using ConsultingKoiFish.BLL.DTOs.BlogDTOs;
using ConsultingKoiFish.BLL.DTOs.BlogImageDTOs;
using ConsultingKoiFish.BLL.DTOs.PackageImageDTOs;
using ConsultingKoiFish.BLL.Services.Implements;
using ConsultingKoiFish.BLL.Services.Interfaces;
using ConsultingKoiFish.DAL.Entities;
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

        public AdvertisementPackagesController(IAdvertisementPackageService advertisementPackageService)
        {
            _advertisementPackageService = advertisementPackageService;
        }
        
        [Authorize]
        [HttpPost]
        [Route("create-update-advertisement-package")]
        public async Task<IActionResult> CreateUpdateAdvertisementPackage(AdvertisementPackageRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid) return ModelInvalid();
                if(dto.ImageIds.Contains(0))
                {
                    ModelState.AddModelError("ImageIds", "Id ảnh phải là số nguyên dương.");
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

		[Authorize]
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
	}
}