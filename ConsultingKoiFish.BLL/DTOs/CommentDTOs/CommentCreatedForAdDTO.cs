using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.BLL.DTOs.CommentDTOs
{
	public class CommentCreatedForAdDTO
	{
		public int AdvertisementId { get; set; }
		public string Content { get; set; } = null!;
		public DateTime CreatedDate { get; set; }

		public CommentCreatedForAdDTO(CommentForAdRequestDTO requestDTO)
		{
			AdvertisementId = requestDTO.AdvertisementId;
			Content = requestDTO.Content;
			CreatedDate = DateTime.Now;
		}
	}
}
