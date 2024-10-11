using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.BLL.DTOs.CommentDTOs
{
	public class CommentCreateDTO
	{
		public int? BlogId { get; set; }
		public int? AdvertisementId { get; set; }
		public string Content { get; set; } = null!;
		public DateTime CreatedDate { get; set; }

        public CommentCreateDTO(CommentRequestDTO requestDTO)
        {
            BlogId = requestDTO.BlogId;
			AdvertisementId = requestDTO.AdvertisementId;
			Content = requestDTO.Content;
			CreatedDate = DateTime.Now;
        }
    }
}
