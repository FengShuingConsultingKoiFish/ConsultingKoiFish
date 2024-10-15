using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.BLL.DTOs.UserDetailDTOs
{
	public class UserDetailGetListDTO
	{
		[Required(ErrorMessage = "Vui lòng nhập số trang.")]
		public int PageIndex { get; set; }
		[Required(ErrorMessage = "Vui lòng nhập kích cỡ trang.")]
		public int PageSize { get; set; }
        public string? FullName { get; set; }
        public string? UserName { get; set; }
        public string? UserId { get; set; }
    }
}
