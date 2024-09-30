using ConsultingKoiFish.BLL.DTOs.AccountDTOs;
using ConsultingKoiFish.BLL.DTOs.Response;
using Microsoft.AspNetCore.Identity;

namespace ConsultingKoiFish.BLL.Services.Implements;

public interface IAccountService
{
    public Task<AccountViewDTO> SignUpAsync(AccountCreateRequestDTO accRequest);
    public Task<AuthenResultDTO> SignInAsync(AuthenDTO authenDTO);
    public Task<AuthenResultDTO> GenerateTokenAsync(IdentityUser user);
    public Task<BaseResponse> SendEmailConfirmation(IdentityUser user);
    public Task<BaseResponse> SendOTP2FA(IdentityUser user, string password);
    public Task<BaseResponse> SignOutAsync(SignOutDTO signOutDTO);
    public Task<BaseResponse> CheckToRenewTokenAsync(AuthenResultDTO authenResult);
    public Task<BaseResponse> ForgotPasswordAsync(AccountForgotPasswordDTO dto);
    public Task<BaseResponse> ResetPasswordAsync(AccountResetpassDTO dto);
}