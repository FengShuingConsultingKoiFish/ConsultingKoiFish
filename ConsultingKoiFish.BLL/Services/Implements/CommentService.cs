using AutoMapper;
using ConsultingKoiFish.BLL.DTOs.CommentDTOs;
using ConsultingKoiFish.BLL.DTOs.ImageDTOs;
using ConsultingKoiFish.BLL.DTOs.Response;
using ConsultingKoiFish.BLL.Services.Interfaces;
using ConsultingKoiFish.DAL.Entities;
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

		public async Task<BaseResponse> CreateUpdateComment(CommentRequestDTO dto, string userId)
		{
			try
			{
				await _unitOfWork.BeginTransactionAsync();
				var repo = _unitOfWork.GetRepo<Comment>();

				var any = await repo.AnyAsync(new QueryBuilder<Comment>()
										.WithPredicate(x => x.Id == dto.Id)
										.Build());
				if (any)
				{
					Comment comment = new Comment();
					var blogComment = await repo.GetSingleAsync(new QueryBuilder<Comment>()
					.WithPredicate(x => x.Id == dto.Id && x.BlogId == dto.BlogId)
					.Build());
					if (blogComment == null)
					{
						comment = await repo.GetSingleAsync(new QueryBuilder<Comment>()
					.WithPredicate(x => x.Id == dto.Id && x.AdvertisementId == dto.AdvertisementId)
					.Build());
					}
					else
					{
						comment = blogComment;
					}
					if (!comment.UserId.Equals(userId)) return new BaseResponse { IsSuccess = false, Message = "Bình luận không phải của người dùng." };
					var updatedCommentDto = new CommentUpdateDTO { Content = comment.Content };
					var updatedComment = _mapper.Map(updatedCommentDto, comment);
					await repo.UpdateAsync(updatedComment);
				}
				else
				{
					var createdCommentDto = new CommentCreateDTO(dto);
					var createdComment = _mapper.Map<Comment>(createdCommentDto);
					createdComment.UserId = userId;
					await repo.CreateAsync(createdComment);
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
	}
}
