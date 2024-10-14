using ConsultingKoiFish.DAL.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.BLL.DTOs.PurchasedPackageDTOs
{
	public class PurchasedPackageGetListDTO
	{
		[Required(ErrorMessage = "Vui lòng nhập số trang.")]
		public int PageIndex { get; set; }
		[Required(ErrorMessage = "Vui lòng nhập kích cỡ trang.")]
		public int PageSize { get; set; }
        public PurchasedPackageStatus? Status { get; set; }
        public OrderImage? OrderImage { get; set; }
        public OrderDate? OrderDate { get; set; }

		public bool IsValidStatus()
		{
			if (Status.HasValue)
				return Enum.IsDefined(typeof(PurchasedPackageStatus), Status);
			return true;
		}

		public bool IsValidOrderImage()
		{
			if (OrderImage.HasValue)
				return Enum.IsDefined(typeof(OrderImage), OrderImage);
			return true;
		}

		public bool IsValidOrderDate()
		{
			if (OrderDate.HasValue)
				return Enum.IsDefined(typeof(OrderDate), OrderDate);
			return true;
		}
    }
}
