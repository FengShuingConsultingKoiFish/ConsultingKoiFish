using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.DAL.Entities
{
	public class ApplicationUser : IdentityUser
	{
		public virtual UserDetail UserDetail { get; set; }
		public virtual UserZodiac UserZodiac { get; set; }
		public virtual ICollection<UserPond> UserPonds { get; set; } = new List<UserPond>();
		public virtual ICollection<Image> Images { get; set; } = new List<Image>();
		public virtual ICollection<Blog> Blogs { get; set; } = new List<Blog>();
	}
}
