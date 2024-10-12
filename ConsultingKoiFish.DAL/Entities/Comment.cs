using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.DAL.Entities
{
	public partial class Comment
	{
		public int Id { get; set; }
		public string UserId { get; set; }
		public string Content { get; set; } = null!;
		public DateTime CreatedDate { get; set; }

		public virtual ApplicationUser User { get; set; }
		public virtual ICollection<BlogComment> BlogComments { get; set; } = new List<BlogComment>();
		public virtual ICollection<AdComment> AdComments { get; set; } = new List<AdComment>();
	}
}
