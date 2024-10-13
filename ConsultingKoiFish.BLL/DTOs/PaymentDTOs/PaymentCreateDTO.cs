using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.BLL.DTOs.PaymentDTOs
{
	public class PaymentCreateDTO
	{
		public int Id { get; set; }
		public string UserId { get; set; }
		public int AdvertisementPackageId { get; set; }
		public long TransactionId { get; set; }
		public string? Content { get; set; }
        public double Amount { get; set; }
        public DateTime CreatedDate { get; set; }
	}
}
