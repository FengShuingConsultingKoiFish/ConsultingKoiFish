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
			return Enum.IsDefined(typeof(BlogStatus), AdvertisementStatus);
		}

		public bool IsValidOrderAdvertisement()
		{
			return Enum.IsDefined(typeof(OrderAdvertisement), OrderAdvertisement);
		}

		public bool IsValidOrderComment()
		{
			return Enum.IsDefined(typeof(OrderComment), OrderComment);
		}

		public bool IsValidOrderImage()
		{
			return Enum.IsDefined(typeof(OrderImage), OrderImage);
		}
	}
}
