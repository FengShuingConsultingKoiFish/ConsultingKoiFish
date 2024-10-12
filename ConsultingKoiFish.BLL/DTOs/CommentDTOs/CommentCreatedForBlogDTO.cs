using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.BLL.DTOs.CommentDTOs
{
	public class CommentCreatedForBlogDTO
	{
		public int BlogId { get; set; }
		public string Content { get; set; } = null!;
		public DateTime CreatedDate { get; set; }

        public CommentCreatedForBlogDTO(CommentForBlogRequestDTO requestDTO)
        {
            BlogId = requestDTO.BlogId;
			Content = requestDTO.Content;
			CreatedDate = DateTime.Now;
        }
    }
}
