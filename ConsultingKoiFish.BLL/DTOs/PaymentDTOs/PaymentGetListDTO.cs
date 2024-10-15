using ConsultingKoiFish.DAL.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.BLL.DTOs.PaymentDTOs
{
	public class PaymentGetListDTO
	{
        [Required(ErrorMessage = "Số trang không được để trống.")]
        public int PageIndex { get; set; }
		[Required(ErrorMessage = "Kích thước trang không được để trống.")]
		public int PageSize { get; set; }
        public OrderDate? OrderDate { get; set; }
        public long? TransactionId { get; set; }
        public OrderImage? OrderImage { get; set; }

        public bool IsValidOrderDate()
        {
            if (OrderDate.HasValue) return Enum.IsDefined(typeof(OrderDate), OrderDate);
            return true;
        }

        public bool IsValidOrderImage()
        {
            if (OrderImage.HasValue) return Enum.IsDefined(typeof(OrderImage), OrderImage);
            return true;
        }
    }
}
