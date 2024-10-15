using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.DAL.Entities
{
	public partial class UserDetail
	{
		public Guid Id { get; set; }

        public string UserId {  get; set; }

        [AllowNull]
        public string? FullName { get; set; }
		[AllowNull]
		public string? IdentityCard { get; set; }
		[AllowNull]
		public DateTime? DateOfBirth { get; set; }
		[AllowNull]
		public string? Gender { get; set; }
		[AllowNull]
		public string? Avatar { get; set; }
		[AllowNull]
		public string? Status { get; set; }
		[AllowNull]
		public bool? IsActive { get; set; }
		[AllowNull]
		public DateTime? CreatedDate { get; set; }
        //
        public virtual ApplicationUser User { get; set; }
    }
}
