using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.BLL.DTOs.AdvertisementPackageDTOs
{
	public class AdvertismentPackagePurchaseDTO
	{
		public int PackageId { get; set; }
		public string Name { get; set; } = null!;
		public double Price { get; set; }
	}
}
