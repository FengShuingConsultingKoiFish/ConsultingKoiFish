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
	public class CommentForBlogRequestDTO
	{
		public int CommentId { get; set; }
		[Required(ErrorMessage = "Blog không được để trống.")]
		public int BlogId { get; set; }
		public string Content { get; set; } = null!;
	}
}
