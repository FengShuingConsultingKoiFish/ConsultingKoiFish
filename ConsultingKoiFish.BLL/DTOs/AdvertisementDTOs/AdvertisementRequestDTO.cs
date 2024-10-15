using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.BLL.DTOs.AdvertisementDTOs
{
	public class AdvertisementRequestDTO
	{
		public int Id { get; set; }
		[Required(ErrorMessage = "Hãy chọn gói quảng cáo bạn sẽ dùng để đăng quảng cáo này.")]
		public int PurchasedPackageId { get; set; }
		public string Title { get; set; } = null!;
		[Required(ErrorMessage = "Phần mô tả không được để trống.")]
		public string? Description { get; set; }
		public double Price { get; set; }
        public List<int>? ImageIds { get; set; }
    }
}
