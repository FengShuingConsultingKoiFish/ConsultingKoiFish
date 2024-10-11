using ConsultingKoiFish.DAL.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.BLL.DTOs.CommentDTOs
{
	public class CommentRequestDTO
	{
		public int Id { get; set; }
		[Required(ErrorMessage = "Hãy lựa chọn bình luận cho loại nào. Blog = 1/Ad = 2")]
		public int? BlogId { get; set; }
		public int? AdvertisementId { get; set; }
		public string Content { get; set; } = null!;
	}
}
