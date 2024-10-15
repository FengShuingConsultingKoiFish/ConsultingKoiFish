using ConsultingKoiFish.BLL.DTOs.CommentDTOs;
using ConsultingKoiFish.BLL.DTOs.ImageDTOs;
using ConsultingKoiFish.BLL.DTOs.Response;
using ConsultingKoiFish.DAL.Entities;
using ConsultingKoiFish.DAL.Enums;
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
		Task<BaseResponse> CreateUpdateCommentForAd(CommentForAdRequestDTO dto, string userId);
		Task<BaseResponse> DeleteComment(int commentId, string userId);
		Task<List<CommentViewDTO>> ConvertSpeciedCommentToCommentViews<TComment>(ICollection<TComment> comments,
																				Func<TComment, int> getCommentId);
		List<CommentViewDTO> ConvertToCommentViews(ICollection<Comment> comments);
	}
}
