using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.BLL.DTOs.AdImageDTOs
{
	public class AdImageRequestDTO
	{
		[Required(ErrorMessage = "AdvertisementId không được để trống.")]
		public int AdvertisementId { get; set; }

		public List<int>? ImagesId { get; set; } = new List<int>();
	}
}
