using AutoMapper;
using ConsultingKoiFish.BLL.DTOs.AdvertisementPackageDTOs;
using ConsultingKoiFish.BLL.DTOs.BlogDTOs;
using ConsultingKoiFish.BLL.DTOs.PaymentDTOs;
using ConsultingKoiFish.BLL.DTOs.Response;
using ConsultingKoiFish.BLL.Services.Interfaces;
using ConsultingKoiFish.DAL.Entities;
using ConsultingKoiFish.DAL.Enums;
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
		private readonly IAdvertisementPackageService _advertisementPackageService;

		public PaymentService(IUnitOfWork unitOfWork, IMapper mapper, IAdvertisementPackageService advertisementPackageService)
        {
			this._unitOfWork = unitOfWork;
			this._mapper = mapper;
			this._advertisementPackageService = advertisementPackageService;
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
																	.WithPredicate(x => x.AdvertisementPackageId == dto.AdvertisementPackageId && x.TransactionId == dto.TransactionId)
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


		#region Admin

		public async Task<PaginatedList<PaymentViewDTO>> GetAllPaymentsForAdmin(PaymentGetListDTO dto)
		{
			var repo = _unitOfWork.GetRepo<Payment>();
			var imageRepo = _unitOfWork.GetRepo<Image>();
			var loadedRecords = repo.Get(new QueryBuilder<Payment>()
				.WithTracking(false)
				.WithInclude(x => x.User, r => r.AdvertisementPackage)
				.Build());

			if(dto.OrderDate.HasValue)
			{
				switch((int)dto.OrderDate)
				{
					case 1: loadedRecords = loadedRecords.OrderByDescending(x => x.CreatedDate); break;
					case 2: loadedRecords = loadedRecords.OrderBy(x => x.CreatedDate); break;
				}
			}

			if(dto.TransactionId.HasValue)
			{
				loadedRecords = loadedRecords.Where(x => x.TransactionId == dto.TransactionId);
			}

			var pagedRecords = await PaginatedList<Payment>.CreateAsync(loadedRecords, dto.PageIndex, dto.PageSize);
			var response = await ConvertPaymentsToPaymentViews(pagedRecords, dto.OrderImage);
			return new PaginatedList<PaymentViewDTO>(response, pagedRecords.TotalItems, dto.PageIndex, dto.PageSize);
		}


		public async Task<PaymentViewDTO> GetPaymentByIdForAdmin(int id, OrderImage? orderImage)
		{
			var repo = _unitOfWork.GetRepo<Payment>();
			var payment = await repo.GetSingleAsync(new QueryBuilder<Payment>()
				.WithPredicate(x => x.Id == id)
				.WithTracking(false)
				.WithInclude(x => x.User, r => r.AdvertisementPackage)
				.Build());
			if (payment == null) return null;
			var response = await ConvertPaymentToPaymentView(payment, orderImage);
			return response;
		}
		#endregion

		#region Member

		public async Task<PaginatedList<PaymentViewDTO>> GetAllPaymentsForMember(PaymentGetListDTO dto, string userId)
		{
			var repo = _unitOfWork.GetRepo<Payment>();
			var imageRepo = _unitOfWork.GetRepo<Image>();
			var loadedRecords = repo.Get(new QueryBuilder<Payment>()
				.WithPredicate(x => x.UserId.Equals(userId))
				.WithTracking(false)
				.WithInclude(x => x.User, r => r.AdvertisementPackage)
				.Build());
			var pagedRecords = await PaginatedList<Payment>.CreateAsync(loadedRecords, dto.PageIndex, dto.PageSize);
			var response = await ConvertPaymentsToPaymentViews(pagedRecords, dto.OrderImage);
			return new PaginatedList<PaymentViewDTO>(response, pagedRecords.TotalItems, dto.PageIndex, dto.PageSize);
		}

		public async Task<PaymentViewDTO> GetPaymentByIdForMember(int id, string userId, OrderImage? orderImage)
		{
			var repo = _unitOfWork.GetRepo<Payment>();
			var payment = await repo.GetSingleAsync(new QueryBuilder<Payment>()
				.WithPredicate(x => x.Id == id && x.UserId.Equals(userId))
				.WithTracking(false)
				.WithInclude(x => x.User, r => r.AdvertisementPackage)
				.Build());
			if (payment == null) return null;
			var response = await ConvertPaymentToPaymentView(payment, orderImage);
			return response;
		}

		#endregion

		#region PRIVATE

		/// <summary>
		/// This is used to convert a payment to paymentView
		/// </summary>
		/// <param name="payment"></param>
		/// <param name="orderImage"></param>
		/// <returns></returns>
		private async Task<PaymentViewDTO> ConvertPaymentToPaymentView(Payment payment, OrderImage? orderImage)
		{
			var adPackageViewDto = await _advertisementPackageService.ConvertPackageToPackageView(payment.AdvertisementPackage, orderImage);
			var response = new PaymentViewDTO(payment, adPackageViewDto);
			return response;
		}


		/// <summary>
		/// This is used to convert a collection of payments to paymentViewDtos
		/// </summary>
		/// <param name="payments"></param>
		/// <param name="orderImage"></param>
		/// <returns></returns>
		private async Task<List<PaymentViewDTO>> ConvertPaymentsToPaymentViews(List<Payment> payments, OrderImage? orderImage)
		{
			var response = new List<PaymentViewDTO>();
			foreach (var payment in payments)
			{
				var adPackageViewDto = await _advertisementPackageService.ConvertPackageToPackageView(payment.AdvertisementPackage, orderImage);
				var childResponse = new PaymentViewDTO(payment, adPackageViewDto);
				response.Add(childResponse);
			}
			return response;
		}

		#endregion
	}
}
