using ConsultingKoiFish.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.BLL.DTOs.CommentDTOs
{
	public class CommentViewDTO
	{
		public int Id { get; set; }
		public string UserName { get; set; }
		public string Content { get; set; } = null!;
		public string CreatedDate { get; set; }

        public CommentViewDTO(Comment comment)
        {
            Id = comment.Id;
			UserName = comment.User.UserName;
			Content = comment.Content;
			CreatedDate = comment.CreatedDate.ToString("dd/MM/yyyy");
        }
    }
}
