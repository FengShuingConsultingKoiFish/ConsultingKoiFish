using System;
using System.Collections.Generic;
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
		public string? Description { get; set; }
		public int Limit { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<PurchasedPackage> PurchasedPackages { get; set; } = new List<PurchasedPackage>();
		public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
	}
}
