﻿using AutoMapper;
using ConsultingKoiFish.BLL.DTOs.CommentDTOs;
using ConsultingKoiFish.BLL.DTOs.ImageDTOs;
using ConsultingKoiFish.BLL.DTOs.Response;
using ConsultingKoiFish.BLL.Services.Interfaces;
using ConsultingKoiFish.DAL.Entities;
using ConsultingKoiFish.DAL.Enums;
using ConsultingKoiFish.DAL.Queries;
using ConsultingKoiFish.DAL.UnitOfWork;
using Mailjet.Client.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.BLL.Services.Implements
{
	public class CommentService : ICommentService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public CommentService(IUnitOfWork unitOfWork, IMapper mapper)
		{
			this._unitOfWork = unitOfWork;
			this._mapper = mapper;
		}

		public async Task<BaseResponse> CreateUpdateCommentForAd(CommentForAdRequestDTO dto, string userId)
		{
			try
			{
				await _unitOfWork.BeginTransactionAsync();
				var repo = _unitOfWork.GetRepo<Comment>();
				var adCommentRepo = _unitOfWork.GetRepo<AdComment>();
				var any = await repo.AnyAsync(new QueryBuilder<Comment>()
										.WithPredicate(x => x.Id == dto.CommentId)
										.Build());
				if (any)
				{
					var adComment = await adCommentRepo.GetSingleAsync(new QueryBuilder<AdComment>()
															.WithPredicate(x => x.CommentId == dto.CommentId && x.AdvertisementId == dto.AdvertisementId)
															.WithInclude(x => x.Comment)
															.Build());
					if (adComment == null) return new BaseResponse { IsSuccess = false, Message = "Comment không thuộc về quảng cáo." };
					if (!adComment.Comment.UserId.Equals(userId))
						return new BaseResponse { IsSuccess = false, Message = "Comment không thuộc sở hữu người dùng." };
					var comment = await repo.GetSingleAsync(new QueryBuilder<Comment>()
															.WithPredicate(x => x.Id == dto.CommentId)
															.Build());
					var updatedCommentDTO = new CommentUpdateDTO { Content = dto.Content };
					var updatedComment = _mapper.Map(updatedCommentDTO, comment);
					await repo.UpdateAsync(updatedComment);
				}
				else
				{
					var createdCommentDto = new CommentCreatedForAdDTO(dto);
					var createdComment = _mapper.Map<Comment>(createdCommentDto);
					createdComment.UserId = userId;
					await repo.CreateAsync(createdComment);
					await _unitOfWork.SaveChangesAsync();

					var createdAdComment = new AdComment
					{
						AdvertisementId = dto.AdvertisementId,
						CommentId = createdComment.Id,
					};
					await adCommentRepo.CreateAsync(createdAdComment);
				}
				var saver = await _unitOfWork.SaveAsync();
				await _unitOfWork.CommitTransactionAsync();
				if (!saver)
				{
					return new BaseResponse { IsSuccess = false, Message = "Lưu dữ liệu không thành công." };
				}
				return new BaseResponse { IsSuccess = true, Message = "Lưu dữ liệu thành công." };
			}
			catch (Exception)
			{
				await _unitOfWork.RollBackAsync();
				throw;
			}
		}

		public async Task<BaseResponse> CreateUpdateCommentForBlog(CommentForBlogRequestDTO dto, string userId)
		{
			try
			{
				await _unitOfWork.BeginTransactionAsync();
				var repo = _unitOfWork.GetRepo<Comment>();
				var blogCommentRepo = _unitOfWork.GetRepo<BlogComment>();
				var any = await repo.AnyAsync(new QueryBuilder<Comment>()
										.WithPredicate(x => x.Id == dto.CommentId)
										.Build());
				if (any)
				{
					var blogComment = await blogCommentRepo.GetSingleAsync(new QueryBuilder<BlogComment>()
															.WithPredicate(x => x.CommentId == dto.CommentId && x.BlogId == dto.BlogId)
															.WithInclude(x => x.Comment)
															.Build());
					if(blogComment == null) return new BaseResponse { IsSuccess = false, Message = "Comment không thuộc về blog."};
					if (!blogComment.Comment.UserId.Equals(userId))
						return new BaseResponse { IsSuccess = false, Message = "Comment không thuộc sở hữu người dùng." };
					var comment = await repo.GetSingleAsync(new QueryBuilder<Comment>()
															.WithPredicate(x => x.Id == dto.CommentId)
															.Build());
					var updatedCommentDTO = new CommentUpdateDTO { Content = dto.Content };
					var updatedComment = _mapper.Map(updatedCommentDTO, comment);
					await repo.UpdateAsync(updatedComment);
				}
				else
				{
					var createdCommentDto = new CommentCreatedForBlogDTO(dto);
					var createdComment = _mapper.Map<Comment>(createdCommentDto);
					createdComment.UserId = userId;
					await repo.CreateAsync(createdComment);
					await _unitOfWork.SaveChangesAsync();

					var createdBlogComment = new BlogComment
					{
						BlogId = dto.BlogId,
						CommentId = createdComment.Id,
					};
					await blogCommentRepo.CreateAsync(createdBlogComment);
				}
				var saver = await _unitOfWork.SaveAsync();
				await _unitOfWork.CommitTransactionAsync();
				if (!saver)
				{
					return new BaseResponse { IsSuccess = false, Message = "Lưu dữ liệu không thành công." };
				}
				return new BaseResponse { IsSuccess = true, Message = "Lưu dữ liệu thành công." };
			}
			catch (Exception)
			{
				await _unitOfWork.RollBackAsync();
				throw;
			}
		}

		public async Task<BaseResponse> DeleteComment(int commentId, string userId)
		{
			try
			{
				await _unitOfWork.BeginTransactionAsync();
				var repo = _unitOfWork.GetRepo<Comment>();
				var any = await repo.AnyAsync(new QueryBuilder<Comment>()
											.WithPredicate(x => x.Id == commentId)
											.Build());
				if (any)
				{
					var comment = await repo.GetSingleAsync(new QueryBuilder<Comment>()
															.WithPredicate(x => x.Id == commentId)
															.WithInclude(x => x.AdComment, r => r.BlogComment)
															.WithTracking(false)
															.Build());
					if (!comment.UserId.Equals(userId)) return new BaseResponse { IsSuccess = false, Message = "Comment không thuộc sở hữu người dùng." };
					if(comment.BlogComment != null)
					{
						await _unitOfWork.GetRepo<BlogComment>().DeleteAsync(comment.BlogComment);
						await _unitOfWork.SaveChangesAsync();
					}
					if (comment.AdComment != null)
					{
						await _unitOfWork.GetRepo<AdComment>().DeleteAsync(comment.AdComment);
						await _unitOfWork.SaveChangesAsync();
					}

					await repo.DeleteAsync(comment);
					var saver = await _unitOfWork.SaveAsync();
					await _unitOfWork.CommitTransactionAsync();
					if (!saver) return new BaseResponse { IsSuccess = false, Message = "Lưu dữ liệu thất bại." };
					return new BaseResponse { IsSuccess = true, Message = "Lưu dữ liệu thành công." };
				}
				return new BaseResponse { IsSuccess = false, Message = "Comment không tồn tại." };
			}
			catch(Exception)
			{
				await _unitOfWork.RollBackAsync();
				throw;
			}
		}

		#region Convert

		/// <summary>
		/// this is used to covert a collection of specified comment to CommentViewDTO
		/// </summary>
		/// <typeparam name="TComment"></typeparam>
		/// <param name="comments"></param>
		/// <param name="getCommentId"></param>
		/// <returns></returns>
		public async Task<List<CommentViewDTO>> ConvertSpeciedCommentToCommentViews<TComment>(ICollection<TComment> comments, Func<TComment, int> getCommentId)
		{
			var commentRepo = _unitOfWork.GetRepo<Comment>();
			var responseComments = new List<CommentViewDTO>();

			foreach (var commentItem in comments)
			{
				var commentId = getCommentId(commentItem);
				var comment = await commentRepo.GetSingleAsync(new QueryBuilder<Comment>()
					.WithPredicate(x => x.Id == commentId)
					.WithInclude(x => x.User)
					.WithTracking(false)
					.Build());

				var childResponseComment = new CommentViewDTO(comment);
				responseComments.Add(childResponseComment);
			}

			return responseComments;
		}

		/// <summary>
		/// this is used to convert a collection of Comment to a colection of CommentViewDTOs
		/// </summary>
		/// <param name="comments"></param>
		/// <returns></returns>
		public List<CommentViewDTO> ConvertToCommentViews(ICollection<Comment> comments)
		{
			var commentRepo = _unitOfWork.GetRepo<Comment>();
			var responseComments = new List<CommentViewDTO>();

			foreach (var commentItem in comments)
			{
				var childResponseComment = new CommentViewDTO(commentItem);
				responseComments.Add(childResponseComment);
			}

			return responseComments;
		}

		#endregion
	}
}
