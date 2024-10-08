using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.DAL.Entities
{
	public partial class AdvertisementPackage
	{
		public int Id { get; set; }
		public string Name { get; set; } = null!;
		public double Price { get; set; }
		[AllowNull]
		public string? Description { get; set; }
		[AllowNull]
		public int? LimitAd { get; set; }
		[AllowNull]
		public int? LimitContent { get; set; }
		[AllowNull]
		public int? LimitImage { get; set; }
        public bool IsActive { get; set; } = true;

        public virtual ICollection<PurchasedPackage> PurchasedPackages { get; set; } = new List<PurchasedPackage>();
        public virtual ICollection<PackageImage> PackageImages { get; set; } = new List<PackageImage>();
		public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
	}
}
