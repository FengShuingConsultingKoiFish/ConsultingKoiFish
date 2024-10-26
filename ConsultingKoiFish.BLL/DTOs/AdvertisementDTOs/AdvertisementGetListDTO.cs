using ConsultingKoiFish.DAL.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.BLL.DTOs.AdvertisementDTOs
{
	public class AdvertisementGetListDTO
	{
		[Required(ErrorMessage = "Vui lòng nhập số trang.")]
		public int PageIndex { get; set; }
		[Required(ErrorMessage = "Vui lòng nhập kích cỡ trang.")]
		public int PageSize { get; set; }
		public string? Title { get; set; }
		public AdvertisementStatus? AdvertisementStatus { get; set; }
		public OrderAdvertisement? OrderAdvertisement { get; set; }
		public OrderComment? OrderComment { get; set; }
		public OrderImage? OrderImage { get; set; }

		public bool IsValidAdvertisementStatus()
		{
			if (AdvertisementStatus.HasValue)
				return Enum.IsDefined(typeof(AdvertisementStatus), AdvertisementStatus);
			return true;
		}

		public bool IsValidOrderAdvertisement()
		{
			if (OrderAdvertisement.HasValue)
				return Enum.IsDefined(typeof(OrderAdvertisement), OrderAdvertisement);
			return true;
		}

		public bool IsValidOrderComment()
		{
			if (OrderComment.HasValue)
				return Enum.IsDefined(typeof(OrderComment), OrderComment);
			return true;
		}

		public bool IsValidOrderImage()
		{
			if (OrderImage.HasValue)
				return Enum.IsDefined(typeof(OrderImage), OrderImage);
			return true;
		}
	}
}
