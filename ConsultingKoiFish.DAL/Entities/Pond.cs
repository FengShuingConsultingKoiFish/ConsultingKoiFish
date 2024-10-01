using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.DAL.Entities
{
	public partial class Pond
	{
		public int Id { get; set; }
		public int PondCategoryId { get; set; }
		public string Name { get; set; } = null!;
		public string? Description { get; set; }
		public string? Image { get; set; }

		public virtual PondCategory PondCategory { get; set; }
		public virtual ICollection<PondZodiac> PondZodiacs { get; set; } = new List<PondZodiac>();
		public virtual ICollection<PondDetail> PondDetails { get; set; } = new List<PondDetail>();
	}
}
