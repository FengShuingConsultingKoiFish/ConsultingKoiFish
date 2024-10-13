using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.BLL.DTOs.ImageDTOs
{
	public class ImageRequestDTO
	{
		public int Id { get; set; }
		[Required(ErrorMessage = "Đường dẫn đến ảnh không được để trống")]

		public string FilePath { get; set; } = null!;
		public string? AltText { get; set; }
	}
}
