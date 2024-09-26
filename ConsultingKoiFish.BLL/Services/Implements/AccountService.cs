using AutoMapper;
using ConsultingKoiFish.BLL.DTOs.AccountDTOs;
using ConsultingKoiFish.BLL.DTOs.EmailDTOs;
using ConsultingKoiFish.BLL.DTOs.Response;
using ConsultingKoiFish.BLL.Services.Interfaces;
using ConsultingKoiFish.DAL.Enums;
using ConsultingKoiFish.DAL.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json.Linq;
using System.Web;
using static System.Net.WebRequestMethods;

namespace ConsultingKoiFish.BLL.Services.Implements;

public class AccountService : IAccountService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
	private readonly IIdentityService _identityService;
	private readonly IEmailService _emailService;

	public AccountService(IIdentityService identityService, IUnitOfWork unitOfWork,
                           IEmailService emailService)
    {
        _unitOfWork = unitOfWork;
        _identityService = identityService;
		_emailService = emailService;
	}
    public async Task<AccountViewDTO> SignUpAsync(AccountCreateRequestDTO accRequest)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            var user = new IdentityUser
            {
                Email = accRequest.EmailAddress,
                UserName = accRequest.UserName,
                PhoneNumber = accRequest.PhoneNumber,
            };

            var createResult = await _identityService.CreateAsync(user, accRequest.Password);
            if (!createResult.Succeeded)
            {
                throw new Exception("Một số lỗi xảy ra trong quá trình đăng kí tài khoản. Vui lòn thử lại sau ít phút.");

			}

            await _identityService.AddToRoleAsync(user, Role.Member.ToString());

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            var emailToken = await _identityService.GenerateEmailConfirmationTokenAsync(user);
			var confirmationLink = $"https://localhost:7166/api/Accounts/VerifyEmail?token={emailToken}&email={user.Email}";
			var message = new EmailDTO
			(
				new string[] { user.Email! },
				"Confirmation Email Link!",
				$@"
<p>- Hệ thống nhận thấy bạn vừa đăng kí với Email: {user.Email}.</p>
<p>- Vui lòng truy cập vào link này để xác thực tài khoản: {confirmationLink!}</p>"
			);
			_emailService.SendEmail(message);
            return new AccountViewDTO
            {
                Id = user.Id,
                EmailAddress = user.Email,
                UserName = user.UserName,
                PhoneNumBer = user.PhoneNumber,
            };
        }
        catch (Exception)
        {
            await _unitOfWork.RollBackAsync();
            return null;
            throw;
        } 
    }
}