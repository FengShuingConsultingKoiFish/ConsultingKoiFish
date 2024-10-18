using ConsultingKoiFish.BLL.DTOs;
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

		/// <summary>
		/// This is used to create or update user profile in4
		/// </summary>
		/// <param name="dto"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("create-update-user-detail")]
		public async Task<IActionResult> CreateUpdateUserDetail(UserDetailRequestDTO dto)
		{
			try
			{
				if (!ModelState.IsValid) return ModelInvalid();

				if (dto.Gender != null)
				{
					if (!dto.Gender.Equals("Nam") && !dto.Gender.Equals("Nữ"))
					{
						ModelState.AddModelError("Gender", "Giới tính chỉ nhận Nam hoặc Nữ.");
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
		
		/// <summary>
		/// This is used to update user avatar
		/// </summary>
		/// <param name="dto"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("update-user-avatar")]
		public async Task<IActionResult> UpdateAvatar(UserDetailUpdateAvatarDTO dto)
		{
			try
			{
				var userId = UserId;
				var response = await _userDetailService.UpdateAvatar(dto, userId);
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
		
		/// <summary>
		/// This is used to remove user avatar
		/// </summary>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("delete-user-avatar")]
		public async Task<IActionResult> DeleteAvatar()
		{
			try
			{
				var userId = UserId;
				var response = await _userDetailService.DeleteAvatar(userId);
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
		
		/// <summary>
		/// This is used to get avatar for current user login
		/// </summary>
		/// <returns></returns>
		[Authorize]
		[HttpGet]
		[Route("get-user-avatar-for-user")]
		public async Task<IActionResult> GetUserAvatarByUserId()
		{
			try
			{
				var response = await _userDetailService.GetUserAvatarByUserId(UserId);
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

		/// <summary>
		/// This is used to get a single user avatar userName
		/// </summary>
		/// <param name="userName"></param>
		/// <returns></returns>
		[Authorize]
		[HttpGet]
		[Route("get-user-avatar-by-userName/{userName}")]
		public async Task<IActionResult> GetUserAvatarByUserId([FromRoute]string userName)
		{
			try
			{
				var response = await _userDetailService.GetUserAvatarByUserName(userName);
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
		

		[Authorize]
		[HttpGet]
		[Route("get-user-detail-for-user")]
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

		[HttpGet]
		[Route("get-user-detail-by-id/{id}")]
		public async Task<IActionResult> GetUserDetailById([FromRoute] Guid id)
		{
			try
			{
				var response = await _userDetailService.GetUserDetailById(id);
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

		[HttpGet]
		[Route("get-user-detail-by-userName/{userName}")]
		public async Task<IActionResult> GetUserDetailById([FromRoute] string userName)
		{
			try
			{
				var response = await _userDetailService.GetUserDetailByUserName(userName);
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


		[HttpPost]
		[Route("get-all-details")]
		public async Task<IActionResult> GetAllUserDetails(UserDetailGetListDTO dto)
		{
			try
			{
				if(!ModelState.IsValid) return ModelInvalid();
				var data = await _userDetailService.GetAllUserDetails(dto);
				var response = new PagingDTO<UserDetailViewDTO>(data);
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

		[Authorize]
		[HttpPost]
		[Route("delete-user-detail")]
		public async Task<IActionResult> DeleteUserDetail()
		{
			try
			{
				var userId = UserId;
				var response = await _userDetailService.DeleteUserDetail(userId);
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
	}
}
