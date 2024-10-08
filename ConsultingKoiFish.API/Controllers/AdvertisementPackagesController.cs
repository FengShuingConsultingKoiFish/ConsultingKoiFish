using ConsultingKoiFish.BLL.DTOs.AdvertisementPackageDTOs;
using ConsultingKoiFish.BLL.DTOs.BlogDTOs;
using ConsultingKoiFish.BLL.Services.Interfaces;
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
    }
}
