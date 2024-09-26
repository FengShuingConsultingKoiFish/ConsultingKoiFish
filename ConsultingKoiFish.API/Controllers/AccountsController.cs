using ConsultingKoiFish.BLL.DTOs.AccountDTOs;
using ConsultingKoiFish.BLL.Services.Implements;
using ConsultingKoiFish.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
		private readonly IConfiguration _configuration;

		public AccountsController(IAccountService accountService, IIdentityService identityService, IConfiguration configuration)
        {
			_accountService = accountService;
			this._identityService = identityService;
			this._configuration = configuration;
		}

		[HttpPost]
		[Route("sign-up")]
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
					ModelState.AddModelError("UserNameOrEmail", "UserNameOrEmail đã tồn tại. Vui lòng chọn một UserNameOrEmail khác.");
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
		[Route("verify-email")]
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

		[HttpPost]
		[Route("authen")]
		public async Task<IActionResult> SignInAsync([FromBody]AuthenDTO authenDTO)
		{
			try
			{
				if(!ModelState.IsValid)
				{
					return ModelInvalid();
				}

				var adminEmail = _configuration["Admin:email"];
				if(authenDTO.UserNameOrEmail.Equals(adminEmail))
				{
					var admin = await _identityService.GetByEmailAsync(adminEmail);
					var adminPass = await _identityService.CheckPasswordAsync(admin, authenDTO.Password);
					if(!adminPass)
					{
						ModelState.AddModelError("Password", "Mật khẩu không đúng.");
						return ModelInvalid();
					}

					var adminToken = await _accountService.GenerateTokenAsync(admin);
					if(adminToken == null)
					{
						return GetUnAuthorized("Đã xảy ra lỗi trong quá trình đăng nhập. Vui lòng thử lại sau ít phút");
					}
					return Success(adminToken, "Chào mừng đến với tài khoản Admin.");
				}

				var user = await _identityService.GetByEmailOrUserNameAsync(authenDTO.UserNameOrEmail);
				if(user == null)
				{
					ModelState.AddModelError("UserNameOrEmail", "Tên đăng nhập hoặc Email không đúng.");
					return ModelInvalid();
				}

				var password = await _identityService.CheckPasswordAsync(user, authenDTO.Password);
				if(!password)
				{
					ModelState.AddModelError("Password", "Mật khẩu không đúng.");
					return ModelInvalid();
				}

				if(!(await _identityService.IsEmailConfirmedAsync(user)))
				{
					var sendEmail = await _accountService.SendEmailConfirmation(user);
					return GetUnAuthorized(sendEmail.Message);
				}

				if(user.TwoFactorEnabled)
				{
					var sendOTP = await _accountService.SendOTP2FA(user, authenDTO.Password);
					return GetSuccess(sendOTP);
				}

				var response = await _identityService.PasswordSignInAsync(user, authenDTO.Password, true, true);
				if(response.IsLockedOut)
				{
					return GetUnAuthorized($"Tài khoản của bạn đã bị khóa vì đăng nhập sai nhiều lần. Khóa đến: {user.LockoutEnd.Value.LocalDateTime}.");
				}
				await _identityService.ResetAccessFailedCountAsync(user);
				var token = await _accountService.GenerateTokenAsync(user);
				if(token == null)
				{
					return Error("Lỗi sinh mã đăng nhập. Vui lòng thử lại sau ít phút.");
				}

				return Success(token, "Đăng nhập thành công.");
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ResetColor();
				return Error("Đã xảy ra lỗi trong quá trình đăng nhập. Vui lòng thử lại sau ít phút");
			}
		}

		[Authorize]
		[HttpPost]
		[Route("authen2fa")]
		public async Task<IActionResult> SignIn2FaAsync([FromBody] Authen2FaDTO authen2FaDTO)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return ModelInvalid();
				}
				var user = await _identityService.GetByEmailAsync(authen2FaDTO.Email);
				if (user == null)
				{
					ModelState.AddModelError("Email", "Email không đúng.");
					return ModelInvalid();
				}

				var signIn = await _identityService.TwoFactorSignInAsync("Email", authen2FaDTO.Code, false, false);
				if (!signIn.Succeeded)
				{
					return GetUnAuthorized($"Mã OTP {authen2FaDTO.Code} không hợp lệ.");
				}

				var token = await _accountService.GenerateTokenAsync(user);
				if (token == null)
				{
					return GetUnAuthorized("Không thể sinh mã đăng nhập. Vui lòng thử lại sau ít phút.");
				}

				return Success(token, "Đăng nhập thành công.");
			}
			catch(Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ResetColor();
				return Error("Đã có lỗi xảy ra trong quá trình đăng nhập. Vui lòng thử lại sau ít phút.");
			}
		}

		[Authorize]
		[HttpPost]
		[Route("enable-2fa")]
		public async Task<IActionResult> EnableTwoFactorAuthentication([FromBody]AuthenDTO authenDTO)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return ModelInvalid();
				}

				var user = await _identityService.GetUserAsync(User);
				if (user == null)
				{
					return GetUnAuthorized("Bạn cần đăng nhập để thực hiện tính năng này.");
				}

				if (user.TwoFactorEnabled)
				{
					return Success(new { TwoFactorEnable = user.TwoFactorEnabled }, "Bạn đã kích hoạt tính năng này rồi");
				}

				var result = await _identityService.SetTwoFactorEnabledAsync(user, true);
				if (result.Succeeded)
				{
					var sendOTP = await _accountService.SendOTP2FA(user, authenDTO.Password);
					return GetSuccess(sendOTP);
				}

				return Error("Xác thực 2 yếu tố không thành công. Vui lòng thử lại sau");
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ResetColor();
				return Error("Đã xảy ra lỗi trong quá trình đăng nhập. Vui lòng thử lại sau ít phút");
			}
		}

		[Authorize]
		[HttpPost]
		[Route("disable-2fa")]
		public async Task<IActionResult> DisableTwoFactorAuthentication([FromBody] AuthenDTO authenDTO)
		{
			if (!ModelState.IsValid)
			{
				return ModelInvalid();
			}

			var user = await _identityService.GetUserAsync(User);
			if (user == null)
			{
				return GetUnAuthorized("Không tìm thấy người dùng.");
			}

			// Xác minh mật khẩu trước khi tắt 2FA
			var checkPassword = await _identityService.CheckPasswordSignInAsync(user, authenDTO.Password, false);
			if (!checkPassword.Succeeded)
			{
				ModelState.AddModelError("Password", "Mật khẩu không đúng");
			}

			// Tắt 2FA
			var result = await _identityService.SetTwoFactorEnabledAsync(user, false);
			if (result.Succeeded)
			{
				return SaveSuccess("Tắt xác thực 2 yếu tố thành công.");
			}

			return SaveError(result.Errors);
		}

		[Authorize]
		[HttpPost]
		[Route("sign-out")]
		public async Task<IActionResult> SignOutAsync([FromBody] SignOutDTO signOutDTO)
		{
			try
			{
				var user = _identityService.GetUserAsync(User);
				if(user == null)
				{
					return GetNotFound("Không tìm thấy người dùng.");
				}

				if(!ModelState.IsValid)
				{
					return ModelInvalid();
				}
				await _identityService.SignOutAsync();
				var response = await _accountService.SignOutAsync(signOutDTO);
				if(!response.IsSuccess)
				{
					return Error(response.Message);
				}
				return SaveSuccess(response);
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ResetColor();
				return Error("Đã xảy ra lỗi trong quá trình đăng xuất. Vui lòng thử lại sau ít phút");
			}
		}
	}
}
