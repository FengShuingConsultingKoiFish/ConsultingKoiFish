using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.DAL.Entities
{
	public partial class Advertisement
	{
		public int Id { get; set; }
		public string UserId { get; set; }
		public string Title { get; set; } = null!;
		public string? Description { get; set; }
		public double Price { get; set; }
		public DateTime CreatedDate { get; set; } = DateTime.Now;

		public virtual ApplicationUser User { get; set; }
		public virtual ICollection<AdAttribute> AdAttributes { get; set; } = new List<AdAttribute>();
	}
}
