using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsultingKoiFish.BLL.DTOs.AdvertisementPackageDTOs;

namespace ConsultingKoiFish.BLL.DTOs.PurchasedPackageDTOs
{
	public class PurchasedPackageCreateDTO
	{
		public string UserId { get; set; }
		public int AdvertisementPackageId { get; set; }
		public int MornitoredQuantity { get; set; }
		public int Status { get; set; }
		public DateTime CreatedDate { get; set; }
	}
}
