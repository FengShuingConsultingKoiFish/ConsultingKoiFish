using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsultingKoiFish.BLL.DTOs.AdvertisementPackageDTOs;
using ConsultingKoiFish.DAL.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace ConsultingKoiFish.BLL.DTOs.PurchasedPackageDTOs
{
	public class PurchasedPackageCreateDTO
	{
		public string UserId { get; set; }
		public int AdvertisementPackageId { get; set; }
		public string Name { get; set; } = null!;
		public double Price { get; set; }
		public string? Description { get; set; }
		public int? LimitAd { get; set; }
		public int? LimitContent { get; set; }
		public int? LimitImage { get; set; }
		public int DurationsInDays { get; set; }
		public int MornitoredQuantity { get; set; }
		public int Status { get; set; }
		public bool IsActive { get; set; }
		public DateTime ExpireDate { get; set; }
		public DateTime CreatedDate { get; set; }

		public PurchasedPackageCreateDTO(AdvertisementPackageViewDTO adPackageView, string userId)
		{
			UserId = userId;
			AdvertisementPackageId = adPackageView.Id;
			Name = adPackageView.Name;
			Price = adPackageView.Price;
			Description = adPackageView.Description;
			LimitAd = adPackageView.LimitAd;
			LimitContent = adPackageView.LimitContent;
			LimitImage = adPackageView.LimitImage;
			DurationsInDays = adPackageView.DurationsInDays;
			Status = (int)PurchasedPackageStatus.Available;
			MornitoredQuantity = 0;
			IsActive = true;
			CreatedDate = DateTime.Now;
			ExpireDate = DateTime.Now.AddDays(adPackageView.DurationsInDays);
		}
	}
}
