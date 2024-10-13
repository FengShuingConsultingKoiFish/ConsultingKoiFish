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
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        [DataType(DataType.Date)]
        public DateTime? FromDate { get; set; }
		[DataType(DataType.Date)]
		public DateTime? ToDate { get; set; }
    }
}
