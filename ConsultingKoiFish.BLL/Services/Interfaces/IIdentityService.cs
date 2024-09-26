using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
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
	}
}
