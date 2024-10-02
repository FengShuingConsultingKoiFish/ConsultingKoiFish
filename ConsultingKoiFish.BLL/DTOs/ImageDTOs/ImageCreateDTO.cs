using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.BLL.DTOs.ImageDTOs
{
	public class ImageCreateDTO
	{
		public string FilePath { get; set; } = null!;
		public string? AltText { get; set; }
	}
}
