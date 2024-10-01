using AutoMapper;
using ConsultingKoiFish.BLL.DTOs.Response;
using ConsultingKoiFish.BLL.DTOs.UserDetailDTOs;
using ConsultingKoiFish.BLL.Services.Interfaces;
using ConsultingKoiFish.DAL.Entities;
using ConsultingKoiFish.DAL.Paging;
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

		public async Task<BaseResponse> DeleteUserDetail(string userId)
		{
			var repo = _unitOfWork.GetRepo<UserDetail>();
			var any = await repo.AnyAsync(new QueryBuilder<UserDetail>()
												.WithPredicate(x => x.UserId.Equals(userId))
												.Build());
			if (any)
			{
				var userDetail = await repo.GetSingleAsync(new QueryBuilder<UserDetail>()
															.WithPredicate(x => x.UserId.Equals(userId))
															.Build());
				if (userId != userDetail.UserId) return new BaseResponse { IsSuccess = false, Message = "Người dùng không khớp." };
				userDetail.IsActive = false;
				await repo.UpdateAsync(userDetail);
				var saver = await _unitOfWork.SaveAsync();
				if (!saver) return new BaseResponse { IsSuccess = false, Message = "Xóa dữ liệu thất bại" };
				return new BaseResponse { IsSuccess = true, Message = "Xóa dữ liệu thành công" };
			}

			return new BaseResponse { IsSuccess = false, Message = "Không tồn tại người dùng." };
		}

		public async Task<PaginatedList<UserDetailViewDTO>> GetAllUserDetails(int pageIndex, int pageSize)
		{
			var repo = _unitOfWork.GetRepo<UserDetail>();
			var loadedRecords = repo.Get(new QueryBuilder<UserDetail>()
										.WithPredicate(x => x.IsActive == true)
										.Build());
			var pagedRecords = await PaginatedList<UserDetail>.CreateAsync(loadedRecords, pageIndex, pageSize);
			var resultDTO = _mapper.Map<List<UserDetailViewDTO>>(pagedRecords);
			return new PaginatedList<UserDetailViewDTO>(resultDTO, pagedRecords.TotalItems, pageIndex, pageSize);
		}

		public async Task<PaginatedList<UserDetailViewDTO>> GetAllUserDetailsByName(int pageIndex, int pageSize, string name)
		{
			var repo = _unitOfWork.GetRepo<UserDetail>();
			var loadedRecords = repo.Get(new QueryBuilder<UserDetail>()
										.WithPredicate(x => x.IsActive == true && x.FullName.Contains(name))
										.Build());
			var pagedRecords = await PaginatedList<UserDetail>.CreateAsync(loadedRecords, pageIndex, pageSize);
			var resultDTO = _mapper.Map<List<UserDetailViewDTO>>(pagedRecords);
			return new PaginatedList<UserDetailViewDTO>(resultDTO, pagedRecords.TotalItems, pageIndex, pageSize);
		}

		public async Task<UserDetailViewDTO> GetUserDetailByUserId(string userId)
		{
			var repo = _unitOfWork.GetRepo<UserDetail>();
			var response = await repo.GetSingleAsync(new QueryBuilder<UserDetail>()
													.WithPredicate(x => x.UserId.Equals(userId))
													.WithTracking(false)
													.Build());
			if (response.IsActive == false) return null;
			return _mapper.Map<UserDetailViewDTO>(response);
		}
	}
}
