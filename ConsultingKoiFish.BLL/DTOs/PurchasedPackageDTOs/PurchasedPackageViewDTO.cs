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
		public int MornitoredQuantity { get; set; }
        public string? UserName { get; set; }
        public PurchasedPackageStatus Status { get; set; }
		public string CreatedDate { get; set; }
        public AdvertisementPackageViewDTO AdvertisementPackageViewDTO { get; set; }
        public PurchasedPackageViewDTO(PurchasedPackage purchasedPackage, AdvertisementPackageViewDTO advertisementPackageViewDTO)
        {
            Id = purchasedPackage.Id;
			MornitoredQuantity = purchasedPackage.MornitoredQuantity;
			UserName = purchasedPackage.User.UserName;
			Status = (PurchasedPackageStatus)purchasedPackage.Status;
			CreatedDate = purchasedPackage.CreatedDate.ToString("dd/MM/yyyy");
			AdvertisementPackageViewDTO = advertisementPackageViewDTO;
        }
    }
}
