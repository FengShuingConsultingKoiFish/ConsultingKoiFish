using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.DAL.Entities
{
	public partial class PondCategory
	{
		public int Id { get; set; }
		public string Name { get; set; } = null!;
		public string? Description { get; set; }

		public virtual ICollection<Pond> Ponds { get; set; } = new List<Pond>();
	}
}
