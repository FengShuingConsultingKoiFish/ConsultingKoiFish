using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.DAL.Entities
{
	public partial class PurchasedPackage
	{
		public int Id { get; set; }
		public string UserId { get; set; }
		public int AdvertisementPackageId { get; set; }
		public int Status { get; set; }

		public virtual ApplicationUser User { get; set; }
		public virtual AdvertisementPackage AdvertisementPackage { get; set; }
	}
}
