using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.BLL.DTOs.VnPayDTOs
{
	public class VnPayResponseDTO
	{
		public bool IsSuccess { get; set; }
		public string PaymentMethod { get; set; }
		public string OrderDescription { get; set; }
		public string OrderId { get; set; }
		public long TransactionId { get; set; }
		public string Token { get; set; }
		public string VnPayResponseCode { get; set; }
		public double Amount { get; set; }
	}
}
