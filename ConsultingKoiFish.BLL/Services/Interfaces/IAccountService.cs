using ConsultingKoiFish.BLL.DTOs.AccountDTOs;
using ConsultingKoiFish.BLL.DTOs.Response;
using ConsultingKoiFish.DAL.Entities;
using Microsoft.AspNetCore.Identity;

namespace ConsultingKoiFish.BLL.Services.Implements;

public interface IAccountService
{
	Task<AccountViewDTO> SignUpAsync(AccountCreateRequestDTO accRequest);
	Task<AuthenResultDTO> SignInAsync(AuthenDTO authenDTO);
	Task<AuthenResultDTO> GenerateTokenAsync(ApplicationUser user);
	Task<BaseResponse> SendEmailConfirmation(ApplicationUser user);
	Task<BaseResponse> SendOTP2FA(ApplicationUser user, string password);
	Task<BaseResponse> SignOutAsync(SignOutDTO signOutDTO);
	Task<BaseResponse> CheckToRenewTokenAsync(AuthenResultDTO authenResult);
	Task<BaseResponse> ForgotPasswordAsync(AccountForgotPasswordDTO dto);
	Task<BaseResponse> ResetPasswordAsync(AccountResetpassDTO dto);
}
