using ConsultingKoiFish.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
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

		public async Task<IdentityUser> GetByEmailAsync(string email)
		{
			var existedUser = await _userManager.FindByEmailAsync(email);
			return existedUser;
		}
	}
}
