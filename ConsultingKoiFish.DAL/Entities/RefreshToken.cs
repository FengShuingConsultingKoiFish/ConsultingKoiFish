using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.DAL.Entities
{
	public partial class RefreshToken
	{
		public Guid Id { get; set; }
		public string UserId { get; set; }
		public string JwtId { get; set; }
		public string Token { get; set; }
		public bool IsUsed { get; set; }
		public bool IsRevoked { get; set; }
		public DateTime IssuedAt { get; set; }
		public DateTime ExpiredAt { get; set; }
		//
		public virtual ApplicationUser User { get; set; }
	}
}
