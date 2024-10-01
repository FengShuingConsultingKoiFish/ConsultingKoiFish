using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.DAL.Entities
{
	public partial class Image
	{
		public int Id { get; set; }
		public string FilePath { get; set; } = null!;
		public string? AltText { get; set; }
		public string UserId { get; set; }
		public DateTime CreatedDate { get; set; } = DateTime.Now;

		public virtual ApplicationUser User { get; set; }
		public virtual ICollection<BlogImage> BlogImages { get; set; } = new List<BlogImage>();
	}
}
