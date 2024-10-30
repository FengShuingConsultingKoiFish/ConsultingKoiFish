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
		public string Name { get; set; } = null!;
		public double Price { get; set; }
		public string? Description { get; set; }
		public int? LimitAd { get; set; }
		public int? LimitContent { get; set; }
		public int? LimitImage { get; set; }
		public int DurationsInDays { get; set; }
		public int MornitoredQuantity { get; set; }
		public int Status { get; set; }
		public DateTime CreatedDate { get; set; }
		public DateTime ExpireDate { get; set; }
		public bool IsActive { get; set; }

		public virtual ApplicationUser User { get; set; }
		public virtual ICollection<Advertisement> Advertisements { get; set; } = new List<Advertisement>();
	}
}
