using ConsultingKoiFish.BLL.DTOs.CommentDTOs;
using ConsultingKoiFish.BLL.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.BLL.Services.Interfaces
{
	public interface ICommentService
	{
		Task<BaseResponse> CreateUpdateCommentForBlog(CommentForBlogRequestDTO dto, string userId);
	}
}
