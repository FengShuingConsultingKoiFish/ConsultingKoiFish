using ConsultingKoiFish.BLL.DTOs.AdvertisementPackageDTOs;
using ConsultingKoiFish.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.BLL.DTOs.PaymentDTOs
{
	public class PaymentViewDTO
	{
        public int Id { get; set; }
        public string UserName { get; set; }
        public string PackageName { get; set; } = null!;
        public long TransactionId { get; set; }
		public string? Content { get; set; }
        public double Amount { get; set; }
        public DateTime CreatedDate { get; set; }

        public PaymentViewDTO(Payment payment)
        {
			Id = payment.Id;
            UserName = payment.User.UserName;
			PackageName = payment.AdvertisementPackage.Name;
			TransactionId = payment.TransactionId;
			Content = payment.Content;
            Amount = payment.Amount;
			CreatedDate = payment.CreatedDate;
        }
    }
}
