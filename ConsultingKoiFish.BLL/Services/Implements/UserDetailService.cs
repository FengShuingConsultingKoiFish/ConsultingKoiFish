using AutoMapper;
using ConsultingKoiFish.BLL.DTOs.Response;
using ConsultingKoiFish.BLL.DTOs.UserDetailDTOs;
using ConsultingKoiFish.BLL.Services.Interfaces;
using ConsultingKoiFish.DAL.Entities;
using ConsultingKoiFish.DAL.Queries;
using ConsultingKoiFish.DAL.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.BLL.Services.Implements
{
	public class UserDetailService : IUserDetailService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public UserDetailService(IUnitOfWork unitOfWork, IMapper mapper)
        {
			this._unitOfWork = unitOfWork;
			this._mapper = mapper;
		}
        public async Task<BaseResponse> CreateUpdateUserDetail(UserDetailRequestDTO dto, string userId)
		{
			try
			{
				var repo = _unitOfWork.GetRepo<UserDetail>();
				await _unitOfWork.BeginTransactionAsync();
				var any = await repo.AnyAsync(new QueryBuilder<UserDetail>()
												.WithPredicate(x => x.UserId.Equals(userId))
												.Build());
				if(any)
				{
					var userDetail = await repo.GetSingleAsync(new QueryBuilder<UserDetail>()
																.WithPredicate(x => x.UserId.Equals(userId))
																.Build());
					if(userId != userDetail.UserId) return new BaseResponse { IsSuccess = false, Message = "Người dùng không khớp."};

					var updateUserDetail = _mapper.Map(dto, userDetail);
					await repo.UpdateAsync(updateUserDetail);
				}
				else
				{
					var userDetailId = new Guid();
					var userDetail = _mapper.Map<UserDetail>(dto);
					userDetail.Id = userDetailId;
					userDetail.UserId = userId;
					userDetail.IsActive = true;
					userDetail.CreatedDate = DateTime.Now;
					await repo.CreateAsync(userDetail);
				}
				var saver = await _unitOfWork.SaveAsync();
				await _unitOfWork.CommitTransactionAsync();
				if (!saver) return new BaseResponse { IsSuccess = false, Message = "Lưu dữ liệu thất bại" };
				return new BaseResponse { IsSuccess = true, Message = "Lưu dữ liệu thành công" };
			}
			catch(Exception)
			{
				await _unitOfWork.RollBackAsync();
				throw;
			}
		}
	}
}
