using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.DAL.Entities
{
	public partial class UserPond
	{
		public int Id { get; set; }
		public string UserId { get; set; }
		public string PondName { get; set; } = null!;
		public int Quantity { get; set; }
		public string? Description { get; set; }
		public string? Image { get; set; }
		public double Score { get; set; }
		public string? ScoreDetail { get; set; }

		public virtual ApplicationUser User { get; set; }
		public virtual ICollection<PondDetail> PondDetails { get; set; } = new List<PondDetail>();
	}
}
