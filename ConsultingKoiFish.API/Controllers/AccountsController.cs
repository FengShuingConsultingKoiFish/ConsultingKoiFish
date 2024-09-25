using ConsultingKoiFish.BLL.DTOs.AccountDTOs;
using ConsultingKoiFish.BLL.Services.Implements;
using ConsultingKoiFish.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace ConsultingKoiFish.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AccountsController : BaseAPIController
	{
		private readonly IAccountService _accountService;
		private readonly IIdentityService _identityService;

		public AccountsController(IAccountService accountService, IIdentityService identityService)
        {
			_accountService = accountService;
			this._identityService = identityService;
		}

		[HttpPost]
		[Route("SignUp")]
		public async Task<IActionResult> SignUpAsync([FromBody] AccountCreateRequestDTO accountCreate)
		{
			try
			{
				if(!ModelState.IsValid)
				{
					return ModelInvalid();
				}

				if(!accountCreate.Password.Equals(accountCreate.ConfirmPassword))
				{
					ModelState.AddModelError("ConfirmPassword", "Nhắc lại mật khẩu không khớp với mật khẩu");
					return ModelInvalid();
				}

				var userEmail = await _identityService.GetByEmailAsync(accountCreate.EmailAddress);
				if (userEmail != null)
				{
					ModelState.AddModelError("EmailAddress", "Email đã tồn tại trong hệ thống. Vui lòng thử một email khác.");
					return ModelInvalid();
				}

				var userName = await _identityService.GetByUserNameAsync(accountCreate.UserName);
				if(userName != null)
				{
					ModelState.AddModelError("UserName", "UserName đã tồn tại. Vui lòng chọn một UserName khác.");
					return ModelInvalid();
				}

				var response = await _accountService.SignUpAsync(accountCreate);

				if (response == null)
				{
					return Error("Đăng kí KHÔNG thành công");
				}

				return Success(response, "Đăng kí tài khoản thành công.");
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.ToString());
				Console.ResetColor();
				return Error("Đã xảy ra lỗi trong quá trình đăng kí. Hãy thử lại sau một ít phút nữa.");
			}
		}

		[HttpGet]
		[Route("VerifyEmail")]
		public async Task<IActionResult> ConfirmEmailAsync(string token, string email)
		{
			var user = await _identityService.GetByEmailAsync(email);
			if(user == null)
			{
				return GetNotFound("Không tìm thấy Email người dùng trong hệ thống.");
			}
			var decodedToken = HttpUtility.UrlDecode(token);
			var response = await _identityService.ConfirmEmailAsync(user, decodedToken.Replace(" ", "+"));

			if (!response.Succeeded)
			{
				return Error("Xác thực KHÔNG thành công.");
			}

			return Success(response, "Xác thực tài khoản thành công.");
		}
	}
}
