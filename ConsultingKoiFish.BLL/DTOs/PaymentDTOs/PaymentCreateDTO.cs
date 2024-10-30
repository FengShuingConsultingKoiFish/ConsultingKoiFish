using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ConsultingKoiFish.BLL.DTOs.AdvertisementPackageDTOs;

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
		public string ClonePackage { get; set; } = null!;
		public DateTime CreatedDate { get; set; }
		public AdvertisementPackageViewDTO SelectedPackage { get; set; }

		/// <summary>
		/// This is used to snapshort a package at the time that payment executed
		/// </summary>
		/// <param name="adPackageView"></param>
		public void SetMetaDataSnapshot(AdvertisementPackageViewDTO adPackageView)
		{
			ClonePackage = JsonSerializer.Serialize(adPackageView);
		}
	}
}
