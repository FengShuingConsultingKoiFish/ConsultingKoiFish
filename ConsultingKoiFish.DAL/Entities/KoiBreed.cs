using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.DAL.Entities
{
	public partial class KoiBreed
	{
		public int Id { get; set; }
		public int KoiCategoryId { get; set; }
		public string Name { get; set; } = null!;
		public string Colors { get; set; }
		public string Pattern { get; set; }
		public string Description { get; set; }
		public string Image { get; set; }

		public virtual KoiCategory KoiCategory { get; set; }
	}
}
