using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.BLL.DTOs.AdImageDTOs
{
	public class AdImageCreateDTO
	{
		[Required(ErrorMessage = "AdvertisementId không được để trống.")]
		public int AdvertisementId { get; set; }
		[Required(ErrorMessage = "ImageId không được để trống.")]
		public int ImageId { get; set; }
	}
}
