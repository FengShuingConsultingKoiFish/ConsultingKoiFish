using ConsultingKoiFish.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.BLL.Services.Implements
{
	public class IdentityService : IIdentityService
	{
		private readonly UserManager<IdentityUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly SignInManager<IdentityUser> _signInManager;

		public IdentityService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager,
								SignInManager<IdentityUser> signInManager)
        {
			this._userManager = userManager;
			this._roleManager = roleManager;
			this._signInManager = signInManager;
		}

		public async Task<IdentityResult> AddToRoleAsync(IdentityUser user, string role)
		{
			var result = await _userManager.AddToRoleAsync(user, role);
			return result;
		}

		public async Task<bool> CheckPasswordAsync(IdentityUser user, string password)
		{
			var result = await _userManager.CheckPasswordAsync(user, password);
			return result;
		}

		public async Task<SignInResult> CheckPasswordSignInAsync(IdentityUser user, string password, bool LockOutOnFailure)
		{
			var result = await _signInManager.CheckPasswordSignInAsync(user, password, LockOutOnFailure);
			return result;
		}

		public async Task<IdentityResult> ConfirmEmailAsync(IdentityUser user, string token)
		{
			var result = await _userManager.ConfirmEmailAsync(user, token);
			return result;
		}

		public async Task<IdentityResult> CreateAsync(IdentityUser user, string password)
		{
			var result = await _userManager.CreateAsync(user, password);
			return result;
		}

		public async Task<string> GenerateEmailConfirmationTokenAsync(IdentityUser user)
		{
			var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
			return token;
		}

		public async Task<string> GeneratePasswordResetTokenAsync(IdentityUser user)
		{
			var token = await _userManager.GeneratePasswordResetTokenAsync(user);
			return token;
		}

		public async Task<string> GenerateTwoFactorTokenAsync(IdentityUser user, string tokenProvider)
		{
			var otp = await _userManager.GenerateTwoFactorTokenAsync(user, tokenProvider);
			return otp;
		}

		public async Task<IdentityUser> GetByEmailAsync(string email)
		{
			var existedUser = await _userManager.FindByEmailAsync(email);
			return existedUser;
		}

		public async Task<IdentityUser> GetByEmailOrUserNameAsync(string input)
		{
			var user = await _userManager.FindByEmailAsync(input);

			if (user == null)
			{
				user = await _userManager.FindByNameAsync(input);
			}

			return user;
		}

		public async Task<IdentityUser> GetByIdAsync(string id)
		{
			var user = await _userManager.FindByIdAsync(id);
			return user;
		}

		public async Task<IdentityUser> GetByUserNameAsync(string userName)
		{
			var user = await _userManager.FindByNameAsync(userName);
			return user;
		}

		public async Task<IList<string>> GetRolesAsync(IdentityUser user)
		{
			var userRoles = await _userManager.GetRolesAsync(user);
			return userRoles;
		}

		public async Task<IdentityUser> GetUserAsync(ClaimsPrincipal principal)
		{
			var user = await _userManager.GetUserAsync(principal);
			return user;
		}

		public async Task<bool> IsEmailConfirmedAsync(IdentityUser user)
		{
			return await _userManager.IsEmailConfirmedAsync(user);
		}

		public async Task<bool> IsLockedOutAsync(IdentityUser user)
		{
			var response = await _userManager.IsLockedOutAsync(user);
			return response;
		}

		public async Task<SignInResult> PasswordSignInAsync(IdentityUser user, string password, bool isPerSistent, bool LockOutOnFailure)
		{
			var result = await _signInManager.PasswordSignInAsync(user, password, isPerSistent, LockOutOnFailure);
			return result;
		}

		public async Task ResetAccessFailedCountAsync(IdentityUser user)
		{
			await _userManager.ResetAccessFailedCountAsync(user);
		}

		public async Task<IdentityResult> ResetPasswordAsync(IdentityUser user, string token, string newPass)
		{
			var result = await _userManager.ResetPasswordAsync(user, token, newPass);
			return result;
		}

		public async Task<IdentityResult> SetTwoFactorEnabledAsync(IdentityUser user, bool enable2Fa)
		{
			var result = await _userManager.SetTwoFactorEnabledAsync(user, enable2Fa);
			return result;
		}

		public async Task SignOutAsync()
		{
			await _signInManager.SignOutAsync();
		}

		public async Task<SignInResult> TwoFactorSignInAsync(string provider, string code, bool isPersistent, bool rememberClient)
		{
			var signIn = await _signInManager.TwoFactorSignInAsync(provider, code, isPersistent, rememberClient);
			return signIn;
		}
	}
}
