using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsultingKoiFish.BLL.Helpers.StringHelpers;

namespace ConsultingKoiFish.BLL.DTOs.ImageDTOs
{
	public class ImageUploadDTO
	{
        [Required(ErrorMessage = "Vui lòng import file path.")]
        public IFormFile File { get; set; } = null!;
        public string? FileName { get; set; }
        
        public void ProcessFileName(bool isCamelCase = false)
        {
	        if (!string.IsNullOrEmpty(FileName))
	        {
		        FileName = StringHelper.ConvertToPascalOrCamelCase(FileName, isCamelCase);
	        }
        }
    }
}
