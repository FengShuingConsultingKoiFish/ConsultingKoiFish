using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.DAL.Entities
{
	public class UserDetail
	{
		public Guid Id { get; set; }

        public string UserId {  get; set; }

        public string FullName { get; set; } = null!;
        public string? IdentityCard { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Avatar { get; set; }
        public string? Status { get; set; }
        public bool IsLocked { get; set; }
        public DateTime CreatedDate { get; set; }
        //
        public virtual IdentityUser User { get; set; }
    }
}
