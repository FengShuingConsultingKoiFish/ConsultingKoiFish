using AutoMapper;
using ConsultingKoiFish.BLL.DTOs.AccountDTOs;
using ConsultingKoiFish.BLL.DTOs.EmailDTOs;
using ConsultingKoiFish.BLL.DTOs.Response;
using ConsultingKoiFish.BLL.Services.Interfaces;
using ConsultingKoiFish.DAL.Entities;
using ConsultingKoiFish.DAL.Enums;
using ConsultingKoiFish.DAL.Queries;
using ConsultingKoiFish.DAL.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using static System.Net.WebRequestMethods;

namespace ConsultingKoiFish.BLL.Services.Implements;

public class AccountService : IAccountService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
	private readonly IIdentityService _identityService;
	private readonly IEmailService _emailService;
	private readonly IConfiguration _configuration;

	public AccountService(IIdentityService identityService, IUnitOfWork unitOfWork,
                           IEmailService emailService, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _identityService = identityService;
		_emailService = emailService;
		this._configuration = configuration;
	}

	public async Task<AuthenResultDTO> GenerateTokenAsync(IdentityUser user)
	{
		try
		{
			var authClaims = new List<Claim>
			{
				new Claim(ClaimTypes.Email, user.Email),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				new Claim(ClaimTypes.NameIdentifier, user.Id),
				new Claim(ClaimTypes.Name, user.UserName)
			};

			var userRoles = await _identityService.GetRolesAsync(user);
			foreach (var role in userRoles)
			{
				authClaims.Add(new Claim(ClaimTypes.Role, role.ToString()));
			}

			var authenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

			var token = new JwtSecurityToken(
					issuer: _configuration["JWT:ValidIssuer"],
					audience: _configuration["JWT:ValidAudience"],
					expires: DateTime.Now.AddMinutes(30),
					claims: authClaims,
					signingCredentials: new SigningCredentials(authenKey, SecurityAlgorithms.HmacSha512)
				);

			var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

			var refreshToken = GenerateRefreshToken();
			var refreshTokenInDb = new RefreshToken
			{
				Id = Guid.NewGuid(),
				JwtId = token.Id,
				UserId = user.Id,
				Token = refreshToken,
				IsUsed = false,
				IsRevoked = false,
				IssuedAt = DateTime.Now,
				ExpiredAt = DateTime.Now.AddDays(1),
			};

			await _unitOfWork.BeginTransactionAsync();

			var refreshTokenRepo = _unitOfWork.GetRepo<RefreshToken>();
			var refreshTokenByIds = await refreshTokenRepo.Get(new QueryBuilder<RefreshToken>()
														.WithPredicate(x => x.UserId.Equals(user.Id))
														.WithTracking(true)
														.Build()).ToListAsync();
			foreach (var item in refreshTokenByIds)
			{
				await refreshTokenRepo.DeleteAsync(item);
			}
			await refreshTokenRepo.CreateAsync(refreshTokenInDb);
			await _unitOfWork.SaveChangesAsync();
			await _unitOfWork.CommitTransactionAsync();

			return new AuthenResultDTO
			{
				Token = accessToken,
				RefreshToken = refreshToken,
			};
		}
		catch (Exception)
		{
			await _unitOfWork.RollBackAsync();
			return null;
			throw;
		}
		
    }

	public async Task<BaseResponse> SendEmailConfirmation(IdentityUser user)
	{
		try
		{
			var emailToken = await _identityService.GenerateEmailConfirmationTokenAsync(user);
			var encodedToken = HttpUtility.UrlEncode(emailToken);
			var confirmationLink = $"https://localhost:7166/api/Accounts/VerifyEmail?token={encodedToken}&email={user.Email}";
			var message = new EmailDTO
			(
				new string[] { user.Email! },
				"Confirmation Email Link!",
				$@"
<p>- Hệ thống nhận thấy bạn vừa đăng kí với Email: {user.Email}.</p>
<p>- Vui lòng truy cập vào link này để xác thực tài khoản: {confirmationLink!}</p>"
			);
			_emailService.SendEmail(message);
			return new BaseResponse { IsSuccess = true, Message = "Tài khoản của bạn chưa được xác thực. Vui lòng xác thực email của bạn để tiếp tục đăng nhập." };
		}
		catch (Exception)
		{
			throw;
		}
	}

	public async Task<BaseResponse> SendOTP2FA(IdentityUser user, string password)
	{
		try
		{
			await _identityService.SignOutAsync();
			await _identityService.PasswordSignInAsync(user, password, true, false);
			var otp = await _identityService.GenerateTwoFactorTokenAsync(user, "Email");
			var message = new EmailDTO
					(
						new string[] { user.Email },
						"OTP Confirmation",
						$@"
<p>- Mã OTP là riêng tư và <b>tuyệt đối không chia sẽ nó cho bất kì ai khác</b>.</p>
<p>- Đây là mã OTP của bạn: {otp}</p>"
					);
			_emailService.SendEmail(message);
			return new BaseResponse
			{
				IsSuccess = true,
				Message = $"Mã OTP đã được gửi đến Email: {user.Email}"
			};
		}
		catch (Exception)
		{
			throw;
		}
	}

	public Task<AuthenResultDTO> SignInAsync(AuthenDTO authenDTO)
	{
		throw new NotImplementedException();
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

			var sendEmail = await SendEmailConfirmation(user);
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

	#region Private
	private string GenerateRefreshToken()
	{
		var random = new byte[32];
		using (var rng = RandomNumberGenerator.Create())
		{
			rng.GetBytes(random);

			return Convert.ToBase64String(random);
		}
	}
	#endregion
}