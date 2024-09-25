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
		public Task<IdentityResult> CreateAsync(IdentityUser user, string password);
		public Task<IdentityResult> AddToRoleAsync(IdentityUser user, string role);
		public Task<string> GenerateEmailConfirmationTokenAsync(IdentityUser user);
		public Task<IdentityResult> ConfirmEmailAsync(IdentityUser user, string token);
	}
}
