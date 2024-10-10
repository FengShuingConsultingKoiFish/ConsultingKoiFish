using AutoMapper;
using ConsultingKoiFish.BLL.DTOs.BlogDTOs;
using ConsultingKoiFish.BLL.DTOs.PaymentDTOs;
using ConsultingKoiFish.BLL.DTOs.Response;
using ConsultingKoiFish.BLL.Services.Interfaces;
using ConsultingKoiFish.DAL.Entities;
using ConsultingKoiFish.DAL.Paging;
using ConsultingKoiFish.DAL.Queries;
using ConsultingKoiFish.DAL.UnitOfWork;
using Mailjet.Client.Resources;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.BLL.Services.Implements
{
	public class PaymentService : IPaymentService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public PaymentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
			this._unitOfWork = unitOfWork;
			this._mapper = mapper;
		}
        public async Task<BaseResponse> CreatePayment(PaymentCreateDTO dto)
		{
			try
			{
				await _unitOfWork.BeginTransactionAsync();
				var repo = _unitOfWork.GetRepo<Payment>();
				var packageRepo = _unitOfWork.GetRepo<AdvertisementPackage>();
				var anyPackage = await packageRepo.AnyAsync(new QueryBuilder<AdvertisementPackage>()
															.WithPredicate(x => x.Id == dto.AdvertisementPackageId)
															.WithTracking(false)
															.Build());
				if(anyPackage)
				{
					var existPackage = await packageRepo.GetSingleAsync(new QueryBuilder<AdvertisementPackage>()
																	.WithPredicate(x => x.Id == dto.AdvertisementPackageId && x.IsActive == true)
																	.WithTracking(false)
																	.Build());
					if (existPackage == null) return new BaseResponse { IsSuccess = false, Message = "Gói quảng cáo không khả dụng." };
					var existedPayment = await repo.GetSingleAsync(new QueryBuilder<Payment>()
																	.WithPredicate(x => x.Id == dto.AdvertisementPackageId && x.TransactionId == dto.TransactionId)
																	.WithTracking(false)
																	.Build());
					if (existedPayment != null) return new BaseResponse { IsSuccess = false, Message = "Giao dịch đã tồn tại, không thể lưu giao dịch."};
					var createdPayment = _mapper.Map<Payment>(dto);
					await repo.CreateAsync(createdPayment);
					var saver = await _unitOfWork.SaveAsync();
					await _unitOfWork.CommitTransactionAsync();
					if (!saver) return new BaseResponse { IsSuccess = false, Message = "Lưu giao dịch thất bại." };
					return new BaseResponse { IsSuccess = true, Message = "Lưu giao dịch thành công." };
				}

				return new BaseResponse { IsSuccess = false, Message = "Gói quảng cáo không tồn tại" };
			}
			catch (Exception)
			{
				await _unitOfWork.RollBackAsync();
				throw;
			}
		}

		#region Member

		public async Task<PaginatedList<PaymentViewDTO>> GetAllPaymentsByUserId(PaymentGetListDTO dto, string userId)
		{
			var repo = _unitOfWork.GetRepo<Payment>();
			var imageRepo = _unitOfWork.GetRepo<Image>();
			var loadedRecords = repo.Get(new QueryBuilder<Payment>()
				.WithPredicate(x => x.UserId.Equals(userId))
				.WithTracking(false)
				.WithInclude(x => x.User, r => r.AdvertisementPackage)
				.Build());
			var pagedRecords = await PaginatedList<Payment>.CreateAsync(loadedRecords, dto.PageIndex, dto.PageSize);
			var response = new List<PaymentViewDTO>();
			foreach (var payment in pagedRecords)
			{
				var childResponse = new PaymentViewDTO(payment);
				response.Add(childResponse);
			}
			return new PaginatedList<PaymentViewDTO>(response, pagedRecords.TotalItems, dto.PageIndex, dto.PageSize);
		}

		public async Task<PaginatedList<PaymentViewDTO>> GetAllPaymentsByUserIdWithDate(PaymentGetListDTO dto, string userId)
		{
			var repo = _unitOfWork.GetRepo<Payment>();
			var imageRepo = _unitOfWork.GetRepo<Image>();
			var loadedRecords = repo.Get(new QueryBuilder<Payment>()
				.WithPredicate(x => x.UserId.Equals(userId))
				.WithTracking(false)
				.WithInclude(x => x.User, r => r.AdvertisementPackage)
				.Build());
			if(dto.FromDate.HasValue)
			{
				loadedRecords = loadedRecords.Where(x => x.CreatedDate >= dto.FromDate.Value);
			}

			if (dto.ToDate.HasValue)
			{
				loadedRecords = loadedRecords.Where(x => x.CreatedDate <= dto.ToDate.Value);
			}
			var pagedRecords = await PaginatedList<Payment>.CreateAsync(loadedRecords, dto.PageIndex, dto.PageSize);
			var response = new List<PaymentViewDTO>();
			foreach (var payment in pagedRecords)
			{
				var childResponse = new PaymentViewDTO(payment);
				response.Add(childResponse);
			}
			return new PaginatedList<PaymentViewDTO>(response, pagedRecords.TotalItems, dto.PageIndex, dto.PageSize);
		}

		#endregion
	}
}
