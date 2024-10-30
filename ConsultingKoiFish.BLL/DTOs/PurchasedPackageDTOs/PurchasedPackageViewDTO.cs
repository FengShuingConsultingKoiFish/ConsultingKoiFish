using ConsultingKoiFish.BLL.DTOs.AdvertisementPackageDTOs;
using ConsultingKoiFish.DAL.Entities;
using ConsultingKoiFish.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.BLL.DTOs.PurchasedPackageDTOs
{
	public class PurchasedPackageViewDTO
	{
		public int Id { get; set; }
		public string Name { get; set; } = null!;
		public double Price { get; set; }
		public string? Description { get; set; }
		public int? LimitAd { get; set; }
		public int? LimitContent { get; set; }
		public int? LimitImage { get; set; }
		public int DurationsInDays { get; set; }

		public int AdvertisementPackageId { get; set; }

		public int MornitoredQuantity { get; set; }
		public string UserId { get; set; } = null!;
		public string? UserName { get; set; }
		public PurchasedPackageStatus Status { get; set; }
		public string CreatedDate { get; set; }
		public PurchasedPackageViewDTO(PurchasedPackage purchasedPackage)
		{
			Id = purchasedPackage.Id;
			MornitoredQuantity = purchasedPackage.MornitoredQuantity;
			UserName = purchasedPackage.User.UserName;
			Status = (PurchasedPackageStatus)purchasedPackage.Status;
			CreatedDate = purchasedPackage.CreatedDate.ToString("dd/MM/yyyy");
			Name = purchasedPackage.Name;
			Price = purchasedPackage.Price;
			Description = purchasedPackage.Description;
			LimitAd = purchasedPackage.LimitAd;
			LimitContent = purchasedPackage.LimitContent;
			LimitImage = purchasedPackage.LimitImage;
			DurationsInDays = purchasedPackage.DurationsInDays;
			UserId = purchasedPackage.UserId;
			AdvertisementPackageId = purchasedPackage.AdvertisementPackageId;
		}
	}
}
