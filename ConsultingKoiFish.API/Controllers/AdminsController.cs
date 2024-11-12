using ConsultingKoiFish.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ConsultingKoiFish.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminsController : BaseAPIController
    {
        private readonly IAdminService _adminService;

        public AdminsController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("get-total-statistics- in-year")]
        public async Task<IActionResult> GetTotalStatistic()
        {
            try
            {
                var response = await _adminService.GetTotalStatisticInYear();
                return GetSuccess(response);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message, ex.StackTrace);
                Console.ResetColor();
                return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút.");
            }
        }
        
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("get-monthly-revenue-in-year")]
        public async Task<IActionResult> GetMonthlyRevenue()
        {
            try
            {
                var response = await _adminService.GetMonthlyRevenue();
                return GetSuccess(response);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message, ex.StackTrace);
                Console.ResetColor();
                return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút.");
            }
        }
    }
}