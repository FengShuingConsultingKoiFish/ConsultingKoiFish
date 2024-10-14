using AutoMapper;
using ConsultingKoiFish.BLL.DTOs.Response;
using ConsultingKoiFish.BLL.DTOs.UserDetailDTOs;
using ConsultingKoiFish.BLL.Services.Interfaces;
using ConsultingKoiFish.DAL.Entities;
using ConsultingKoiFish.DAL.Paging;
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
				var imageRepo = _unitOfWork.GetRepo<Image>();
				await _unitOfWork.BeginTransactionAsync();

				var image = await imageRepo.GetSingleAsync(new QueryBuilder<Image>()
															.WithPredicate(x => x.Id == dto.ImageId && x.IsActive == true)
															.WithTracking(false)
															.Build());
				if (image == null) return new BaseResponse { IsSuccess = false, Message = "Ảnh không tồn tại." };
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
					updateUserDetail.Avatar = image.FilePath;
					await repo.UpdateAsync(updateUserDetail);
				}
				else
				{
					var userDetailId = new Guid();
					var userDetail = _mapper.Map<UserDetail>(dto);
					userDetail.Id = userDetailId;
					userDetail.UserId = userId;
					userDetail.IsActive = true;
					userDetail.Avatar = image.FilePath;
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

		public async Task<PaginatedList<UserDetailViewDTO>> GetAllUserDetails(UserDetailGetListDTO dto)
		{
			var repo = _unitOfWork.GetRepo<UserDetail>();
			var loadedRecords = repo.Get(new QueryBuilder<UserDetail>()
										.WithPredicate(x => x.IsActive == true)
										.WithInclude(x => x.User)
										.WithTracking(false)
										.Build());
			if(!string.IsNullOrEmpty(dto.FullName))
			{
				loadedRecords = loadedRecords.Where(x => x.FullName.Contains(dto.FullName));
			}

			if (!string.IsNullOrEmpty(dto.UserName))
			{
				loadedRecords = loadedRecords.Where(x => x.User.UserName.Contains(dto.UserName));
			}

			if (!string.IsNullOrEmpty(dto.UserId))
			{
				loadedRecords = loadedRecords.Where(x => x.UserId.Contains(dto.UserId));
			}

			var pagedRecords = await PaginatedList<UserDetail>.CreateAsync(loadedRecords, dto.PageIndex, dto.PageSize);
			var resultDTO = ConvertUserDetailsToUserDetailViews(pagedRecords);
			return new PaginatedList<UserDetailViewDTO>(resultDTO, pagedRecords.TotalItems, dto.PageIndex, dto.PageSize);
		}


		public async Task<UserDetailViewDTO> GetUserDetailByUserId(string userId)
		{
			var repo = _unitOfWork.GetRepo<UserDetail>();
			var response = await repo.GetSingleAsync(new QueryBuilder<UserDetail>()
													.WithPredicate(x => x.UserId.Equals(userId) && x.IsActive == true)
													.WithInclude(x => x.User)
													.WithTracking(false)
													.Build());
			if (response == null) return null;
			return ConvertUserDetailToUserDetailView(response);
		}

		public async Task<UserDetailViewDTO> GetUserDetailById(Guid id)
		{
			var repo = _unitOfWork.GetRepo<UserDetail>();
			var response = await repo.GetSingleAsync(new QueryBuilder<UserDetail>()
													.WithPredicate(x => x.Id.Equals(id) && x.IsActive == true)
													.WithInclude(x => x.User)
													.WithTracking(false)
													.Build());
			if (response == null) return null;
			return ConvertUserDetailToUserDetailView(response);
		}

		#region Convert

		/// <summary>
		/// This is used to convert a user profile to user profile view
		/// </summary>
		/// <param name="userDetail"></param>
		/// <returns></returns>
		private UserDetailViewDTO ConvertUserDetailToUserDetailView(UserDetail userDetail)
		{
			var repsonse = new UserDetailViewDTO(userDetail);
			return repsonse;
		}

		/// <summary>
		/// This is used to convert a collection of user profile to a collection of user detail views
		/// </summary>
		/// <param name="userDetails"></param>
		/// <returns></returns>
		private List<UserDetailViewDTO> ConvertUserDetailsToUserDetailViews(List<UserDetail> userDetails)
		{
			var repsonse = new List<UserDetailViewDTO>();
            foreach (var userDetail in userDetails)
            {
				var userDetailViewDto = ConvertUserDetailToUserDetailView(userDetail);
				repsonse.Add(userDetailViewDto);
            }
			return repsonse;
        }

		#endregion
	}
}
