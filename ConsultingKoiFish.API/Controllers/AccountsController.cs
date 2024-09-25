using ConsultingKoiFish.BLL.DTOs.AccountDTOs;
using ConsultingKoiFish.BLL.Services.Implements;
using ConsultingKoiFish.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
				if(ModelState.IsValid)
				{
					return ModelInvalid();
				}

				if(!accountCreate.Password.Equals(accountCreate.ConfirmPassword))
				{
					ModelState.AddModelError("ConfirmPassword", "Nhắc lại mật khẩu không khớp với mật khẩu");
					return ModelInvalid();
				}

				var userEmail = await _identityService.GetByEmailAsynce(accountCreate.EmailAddress);
				if (userEmail != null)
				{
					ModelState.AddModelError("EmailAddress", "Email đã tồn tại trong hệ thống. Vui lòng thử một email khác.");
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
    }
}
