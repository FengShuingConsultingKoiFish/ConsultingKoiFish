using ConsultingKoiFish.DAL.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.BLL.DTOs.ImageDTOs
{
	public class ImageGetListDTO
	{
		[Required(ErrorMessage = "Vui lòng nhập số trang.")]
		public int PageIndex { get; set; }

		[Required(ErrorMessage = "Vui lòng nhập kích cỡ trang.")]
		public int PageSize { get; set; }
        public string? Name { get; set; }
        public OrderDate? OrderDate { get; set; }

		public bool IsValidOrderDate()
		{
			if (OrderDate.HasValue) return Enum.IsDefined(typeof(OrderDate), OrderDate);
			return true;
		}
    }
}
