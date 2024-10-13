using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.DAL.Entities
{
	public partial class AdComment
	{
		public int Id { get; set; }
		public int AdvertisementId { get; set; }
		public int CommentId { get; set; }

		public virtual Advertisement Advertisement { get; set; }
		public virtual Comment Comment { get; set; }
	}
}
