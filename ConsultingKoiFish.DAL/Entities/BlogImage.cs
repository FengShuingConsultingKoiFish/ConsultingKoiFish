using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.DAL.Entities
{
	public partial class BlogImage
	{
		public int Id { get; set; }
		public int BlogId { get; set; }
		public int ImageId { get; set; }

		public virtual Blog Blog { get; set; }
		public virtual Image Image { get; set; }
	}
}
