using ConsultingKoiFish.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.BLL.DTOs.AdvertisementDTOs
{
	public class AdvertisementCreateDTO
	{
		public string UserId { get; set; }
		public int PurchasedPackageId { get; set; }
		public string Title { get; set; } = null!;
		public string? Description { get; set; }
		public double Price { get; set; }
		public DateTime CreatedDate { get; set; }
		public int Status { get; set; }
		public bool IsActive { get; set; } = true;

        public AdvertisementCreateDTO(AdvertisementRequestDTO dto, string userId)
        {
            UserId = userId;
			PurchasedPackageId = dto.PurchasedPackageId;
			Title = dto.Title;
			Description = dto.Description;
			Price = dto.Price;
			CreatedDate = DateTime.Now;
			Status = (int)AdvertisementStatus.Pending;
			IsActive = true;
        }
    }
}
