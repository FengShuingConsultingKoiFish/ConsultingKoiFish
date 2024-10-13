using ConsultingKoiFish.BLL.DTOs.CommentDTOs;
using ConsultingKoiFish.BLL.DTOs.ImageDTOs;
using ConsultingKoiFish.BLL.Services.Implements;
using ConsultingKoiFish.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ConsultingKoiFish.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CommentsController : BaseAPIController
	{
		private readonly ICommentService _commentService;

		public CommentsController(ICommentService commentService)
        {
			this._commentService = commentService;
		}

		#region Common

		/// <summary>
		/// This is used to create or update a comment for blog
		/// </summary>
		/// <param name="dto"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("create-update-comment-for-blog")]
		public async Task<IActionResult> CreateUpdateCommentForBlog([FromBody]CommentForBlogRequestDTO dto)
		{
			try
			{
				var response = await _commentService.CreateUpdateCommentForBlog(dto, UserId);
				if (!response.IsSuccess) return SaveError(response);
				return SaveSuccess(response);
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ResetColor();
				return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút nữa.");
			}
		}


		/// <summary>
		/// This is used to create or update a comment for advertisement
		/// </summary>
		/// <param name="dto"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("create-update-comment-for-advertisement")]
		public async Task<IActionResult> CreateUpdateCommentForAdvertisement([FromBody]CommentForAdRequestDTO dto)
		{
			try
			{
				var response = await _commentService.CreateUpdateCommentForAd(dto, UserId);
				if (!response.IsSuccess) return SaveError(response);
				return SaveSuccess(response);
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ResetColor();
				return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút nữa.");
			}
		}


		/// <summary>
		/// This is used to delete a comment
		/// </summary>
		/// <param name="dto"></param>
		/// <returns></returns>
		[Authorize]
		[HttpDelete]
		[Route("delete-comment/{commentId}")]
		public async Task<IActionResult> DeleteComment([FromRoute]int commentId)
		{
			try
			{
				var response = await _commentService.DeleteComment(commentId, UserId);
				if (!response.IsSuccess) return SaveError(response);
				return SaveSuccess(response);
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ResetColor();
				return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút nữa.");
			}
		}
		#endregion
	}
}
