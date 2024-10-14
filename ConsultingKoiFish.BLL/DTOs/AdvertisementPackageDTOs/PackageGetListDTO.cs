using ConsultingKoiFish.BLL.Helpers.Fillters;
using ConsultingKoiFish.DAL.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.BLL.DTOs.AdvertisementPackageDTOs
{
	public class PackageGetListDTO
	{
		[Required(ErrorMessage = "Vui lòng nhập số trang.")]
		public int PageIndex { get; set; }
		[Required(ErrorMessage = "Vui lòng nhập kích cỡ trang.")]
		public int PageSize { get; set; }
		public string? Name { get; set; }
        public PriceFilter? PriceFilter { get; set; }
        public OrderImage? OrderImage { get; set; }

        public bool IsValidPriceFilter()
		{
			if (PriceFilter.HasValue)
				return Enum.IsDefined(typeof(PriceFilter), PriceFilter);
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
