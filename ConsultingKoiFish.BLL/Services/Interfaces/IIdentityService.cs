﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.BLL.Services.Interfaces
{
	public interface IIdentityService
	{
		public Task<IdentityUser> GetByEmailAsync(string email);
		public Task<IdentityUser> GetByUserNameAsync(string userName);
		public Task<IdentityResult> CreateAsync(IdentityUser user, string password);
		public Task<IdentityResult> AddToRoleAsync(IdentityUser user, string role);
		public Task<string> GenerateEmailConfirmationTokenAsync(IdentityUser user);
		public Task<IdentityResult> ConfirmEmailAsync(IdentityUser user, string token);
		public Task<bool> CheckPasswordAsync(IdentityUser user, string password);
		public Task<IList<string>> GetRolesAsync(IdentityUser user);
		public Task<IdentityUser> GetByEmailOrUserNameAsync(string input);
		public Task SignOutAsync();
		public Task<SignInResult> PasswordSignInAsync(IdentityUser user, string password, bool isPerSistent, bool LockOutOnFailure);
		public Task<string> GenerateTwoFactorTokenAsync(IdentityUser user, string tokenProvider);
		public Task<SignInResult> TwoFactorSignInAsync(string provider, string code, bool isPersistent, bool rememberClient);
		public Task<bool> IsLockedOutAsync(IdentityUser user);
		public Task<bool> IsEmailConfirmedAsync(IdentityUser user);
		public Task ResetAccessFailedCountAsync(IdentityUser user);
		public Task<IdentityUser> GetUserAsync(ClaimsPrincipal principal);
		public Task<IdentityResult> SetTwoFactorEnabledAsync(IdentityUser user, bool enable2Fa);
		public Task<SignInResult> CheckPasswordSignInAsync(IdentityUser user, string password, bool LockOutOnFailure);
		public Task<IdentityUser> GetByIdAsync(string id);
		public Task<string> GeneratePasswordResetTokenAsync(IdentityUser user);
		public Task<IdentityResult> ResetPasswordAsync(IdentityUser user, string token, string newPass);
	}
}
