using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.DAL.Entities
{
	public partial class AdImage
	{
		public int Id { get; set; }
		public int AdvertisementId { get; set; }
		public int ImageId { get; set; }

		public virtual Advertisement Advertisement { get; set; }
		public virtual Image Image { get; set; }
	}
}
