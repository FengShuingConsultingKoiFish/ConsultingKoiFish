using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.BLL.DTOs.CommentDTOs
{
	public class CommentForAdRequestDTO
	{
		public int CommentId { get; set; }
		[Required(ErrorMessage = "Advertisement không được để trống.")]
		public int AdvertisementId { get; set; }
		public string Content { get; set; } = null!;
	}
}
