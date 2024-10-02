using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.DAL.Entities
{
	public partial class Image
	{
		public int Id { get; set; }
		public string FilePath { get; set; } = null!;
		[AllowNull]
		public string? AltText { get; set; }
		public string UserId { get; set; }
		public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }

        public virtual ApplicationUser User { get; set; }
		public virtual ICollection<BlogImage> BlogImages { get; set; } = new List<BlogImage>();
		public virtual ICollection<AdImage> AdImages { get; set; } = new List<AdImage>();
	}
}
