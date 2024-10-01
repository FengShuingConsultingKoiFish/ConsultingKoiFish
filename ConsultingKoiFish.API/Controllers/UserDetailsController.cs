using ConsultingKoiFish.BLL.DTOs.UserDetailDTOs;
using ConsultingKoiFish.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ConsultingKoiFish.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserDetailsController : BaseAPIController
	{
		private readonly IUserDetailService _userDetailService;

		public UserDetailsController(IUserDetailService userDetailService)
        {
			this._userDetailService = userDetailService;
		}

		[Authorize]
		[HttpPost]
		[Route("create-update-user-detail")]
		public async Task<IActionResult> CreateUpdateUserDetail(UserDetailRequestDTO dto)
		{
			try
			{
				if (!ModelState.IsValid) return ModelInvalid();

				if(dto.Gender != null)
				{
					if(!dto.Gender.Equals("Male") && !dto.Gender.Equals("Female"))
					{
						ModelState.AddModelError("Gender", "Giới tính chỉ nhận Male hoặc Female.");
						return ModelInvalid();
					}
				}

				var userId = UserId;
				var response = await _userDetailService.CreateUpdateUserDetail(dto, userId);
				if (!response.IsSuccess) return SaveError(response.Message);
				return SaveSuccess(response);
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ResetColor();
				return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút.");
			}
		}

		[Authorize]
		[HttpGet]
		[Route("get-user-detail")]
		public async Task<IActionResult> GetUserDetailByUserId()
		{
			try
			{
				var response = await _userDetailService.GetUserDetailByUserId(UserId);
				if (response == null) return GetError();
				return GetSuccess(response);
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ResetColor();
				return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút.");
			}
		}
	}
}
