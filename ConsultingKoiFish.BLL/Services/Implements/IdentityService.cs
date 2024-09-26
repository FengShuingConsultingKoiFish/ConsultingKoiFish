﻿using ConsultingKoiFish.BLL.Services.Interfaces;
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

		public async Task<bool> CheckPasswordAsync(IdentityUser user, string password)
		{
			var result = await _userManager.CheckPasswordAsync(user, password);
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
			var userEmail = await _userManager.FindByEmailAsync(input);
			var username = await _userManager.FindByNameAsync(input);
			if(userEmail == null || username == null)
			{
				return null;
			}

			if(!userEmail.Id.Equals(username.Id))
			{
				return null;
			}

			return userEmail;
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

		public async Task<SignInResult> PasswordSignInAsync(IdentityUser user, string password, bool isPerSistent, bool LockOutOnFailure)
		{
			var result = await _signInManager.PasswordSignInAsync(user, password, isPerSistent, LockOutOnFailure);
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
