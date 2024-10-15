using ConsultingKoiFish.BLL.DTOs.CommentDTOs;
using ConsultingKoiFish.BLL.DTOs.ImageDTOs;
using ConsultingKoiFish.DAL.Entities;
using ConsultingKoiFish.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.BLL.DTOs.AdvertisementDTOs
{
	public class AdvertisementViewDTO
	{
		public int Id { get; set; }
		public string UserName { get; set; }
		public string Title { get; set; } = null!;
		public string? Description { get; set; }
		public double Price { get; set; }
		public string CreatedDate { get; set; }
		public string Status { get; set; }
		public List<ImageViewDTO> ImageViewDtos { get; set; } = new List<ImageViewDTO>();
		public List<CommentViewDTO> CommentViewDtos { get; set; } = new List<CommentViewDTO>();

		public AdvertisementViewDTO(Advertisement advertisement, List<ImageViewDTO> adImageViewDtos, List<CommentViewDTO> adCommentViewDtos)
		{
			Id = advertisement.Id;
			Title = advertisement.Title;
			Description = advertisement.Description;
			UserName = advertisement.User.UserName;
			CreatedDate = advertisement.CreatedDate.ToString("dd/MM/yyyy");
			var statusResponse = (AdvertisementStatus)advertisement.Status;
			Status = statusResponse.ToString();
			ImageViewDtos = adImageViewDtos;
			CommentViewDtos = adCommentViewDtos;
		}
	}
}
