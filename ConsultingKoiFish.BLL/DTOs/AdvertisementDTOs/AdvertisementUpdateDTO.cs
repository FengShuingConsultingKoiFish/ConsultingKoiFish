using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.BLL.DTOs.AdvertisementDTOs
{
	public class AdvertisementUpdateDTO
	{
		public string? Title { get; set; }
		public string? Description { get; set; }
		public double Price { get; set; }

        public AdvertisementUpdateDTO(AdvertisementRequestDTO dto)
        {
            Title = dto.Title;
			Description = dto.Description;
			Price = dto.Price;
        }
    }
}
