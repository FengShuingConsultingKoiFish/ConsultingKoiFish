﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.BLL.DTOs.ImageDTOs
{
	public class ImageViewDTO
	{
		public int Id { get; set; }
		public string FilePath { get; set; } = null!;
		public string? AltText { get; set; }
		public string? UserId { get; set; }
		public string? UserName { get; set; }
		[DataType(DataType.Date)]
		public string? CreatedDate { get; set; }
	}
}
