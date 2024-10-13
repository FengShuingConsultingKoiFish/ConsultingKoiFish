using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.BLL.DTOs.PackageImageDTOs
{
	public class PackageImageDeleteDTO
	{
		public int AdvertisementPackageId { get; set; }
		public List<int> PackageImageIds { get; set; }
	}
}
