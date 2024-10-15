using ConsultingKoiFish.BLL.DTOs.ImageDTOs;
using ConsultingKoiFish.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ConsultingKoiFish.BLL.DTOs.AdvertisementPackageDTOs
{
	public class AdvertisementPackageViewDTO
	{
		public int Id { get; set; }
		public string Name { get; set; } = null!;
		public double Price { get; set; }
		public string? Description { get; set; }
		public int? LimitAd { get; set; }
		public int? LimitContent { get; set; }
		public int? LimitImage { get; set; }
		public bool IsActive { get; set; }
		public string CreatedDate { get; set; }
		public string CreatedBy { get; set; } = null!;
        public List<ImageViewDTO> ImageViewDTOs { get; set; } = new List<ImageViewDTO>();

        public AdvertisementPackageViewDTO(AdvertisementPackage package, List<ImageViewDTO> packageImageViewDtos)
        {
            Id = package.Id;
			Name = package.Name;
			Price = package.Price;
			Description = package.Description;
			LimitAd = package.LimitAd;
			LimitContent = package.LimitContent;
			LimitImage = package.LimitImage;
			IsActive = package.IsActive;
			CreatedDate = package.CreatedDate.ToString("dd/MM/yyyy");
			CreatedBy = package.CreatedBy;
			ImageViewDTOs = packageImageViewDtos;
        }
    }
}
