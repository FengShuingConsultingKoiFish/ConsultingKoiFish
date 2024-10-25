﻿using ConsultingKoiFish.BLL.DTOs.AdvertisementPackageDTOs;
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
        public long TransactionId { get; set; }
		public string? Content { get; set; }
        public double Amount { get; set; }
        public string CreatedDate { get; set; }
        public AdvertisementPackageViewDTO AdvertisementPackageViewDTO { get; set; }

        public PaymentViewDTO(Payment payment, AdvertisementPackageViewDTO advertisementPackageViewDTO)
        {
			Id = payment.Id;
            UserName = payment.User.UserName;
			TransactionId = payment.TransactionId;
			Content = payment.Content;
            Amount = payment.Amount;
			CreatedDate = payment.CreatedDate.ToString("dd/MM/yyyy");
            AdvertisementPackageViewDTO = advertisementPackageViewDTO;
        }
    }
}
