using ConsultingKoiFish.DAL.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.BLL.DTOs.AdvertisementDTOs
{
	public class AdvertisementUpdateStatusDTO
	{
		[Required(ErrorMessage = "AdvertisementId không được để trống.")]
		public int Id { get; set; }
		[Required(ErrorMessage = "Status không được để trống.")]
		public AdvertisementStatus Status { get; set; }

		public bool IsStatusValid()
		{
			return Enum.IsDefined(typeof(AdvertisementStatus), Status);
		}
	}
}
