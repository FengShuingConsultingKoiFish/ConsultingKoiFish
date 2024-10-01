using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.DAL.Entities
{
	public partial class UserZodiac
	{
		public int Id { get; set; }
		public string UserId { get; set; } 
		public int ZodiacId { get; set; }  
		public virtual ApplicationUser User { get; set; }
		public virtual Zodiac Zodiac { get; set; }
	}
}
