using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.BLL.DTOs.AdImageDTOs
{
	public class AdImageDeleteDTO
	{
		[Required(ErrorMessage = "AdvertisementId không thể để trống.")]
		public int AdvertisementId { get; set; }
		[Required(ErrorMessage = "Vui lòng chọn ảnh muốn xóa.")]
		public List<int> ImageIds { get; set; } = new List<int>();
	}
}
