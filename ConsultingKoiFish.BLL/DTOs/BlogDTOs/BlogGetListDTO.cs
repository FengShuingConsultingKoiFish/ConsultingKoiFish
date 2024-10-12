using ConsultingKoiFish.DAL.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.BLL.DTOs.BlogDTOs
{
	public class BlogGetListDTO
	{
        [Required(ErrorMessage = "Vui lòng nhập số trang.")]
        public int PageIndex { get; set; }
		[Required(ErrorMessage = "Vui lòng nhập kích cỡ trang.")]
		public int PageSize { get; set; }
        public string? Title { get; set; }
        public BlogStatus? BlogStatus { get; set; }
        public OrderBlog? OrderBlog { get; set; }
        public OrderComment? OrderComment { get; set; }
        public OrderImage? OrderImage { get; set; }

        public bool IsValidBlogStatus()
        {
            return Enum.IsDefined(typeof(BlogStatus), BlogStatus);
        }

		public bool IsValidOrderBlog()
		{
			return Enum.IsDefined(typeof(OrderBlog), OrderBlog);
		}

        public bool IsValidOrderComment()
        {
			return Enum.IsDefined(typeof(OrderComment), OrderComment);
		}

        public bool IsValidOrderImage()
        {
            return Enum.IsDefined(typeof(OrderImage), OrderImage);
        }
	}
}
