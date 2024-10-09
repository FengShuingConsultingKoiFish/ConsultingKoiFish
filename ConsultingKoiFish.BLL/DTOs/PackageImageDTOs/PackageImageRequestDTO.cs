using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.BLL.DTOs.PackageImageDTOs
{
	public class PackageImageRequestDTO
	{
		[Required(ErrorMessage = "Gói quảng cáo không được để trống.")]
		public int AdvertisementPackageId { get; set; }

		public List<int>? ImagesId { get; set; } = new List<int>();
	}
}
