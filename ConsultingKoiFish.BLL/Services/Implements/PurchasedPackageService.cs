using AutoMapper;
using ConsultingKoiFish.BLL.DTOs.PurchasedPackageDTOs;
using ConsultingKoiFish.BLL.DTOs.Response;
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
	public class PurchasedPackageService : IPurchasedPackageService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public PurchasedPackageService(IUnitOfWork unitOfWork, IMapper mapper)
        {
			this._unitOfWork = unitOfWork;
			this._mapper = mapper;
		}
        public async Task<BaseResponse> CreatePurchasedPacakge(PurchasedPackageCreateDTO dto)
		{
			try
			{
				await _unitOfWork.BeginTransactionAsync();
				var repo = _unitOfWork.GetRepo<PurchasedPackage>();
				var packageRepo = _unitOfWork.GetRepo<AdvertisementPackage>();
				var anyPackage = await packageRepo.AnyAsync(new QueryBuilder<AdvertisementPackage>()
															.WithPredicate(x => x.Id == dto.AdvertisementPackageId)
															.WithTracking(false)
															.Build());
				if (anyPackage)
				{
					var existPackage = await packageRepo.GetSingleAsync(new QueryBuilder<AdvertisementPackage>()
																	.WithPredicate(x => x.Id == dto.AdvertisementPackageId && x.IsActive == true)
																	.WithTracking(false)
																	.Build());
					if (existPackage == null) return new BaseResponse { IsSuccess = false, Message = "Gói quảng cáo không khả dụng." };
					var createdPurchasedPackage = _mapper.Map<PurchasedPackage>(dto);
					await repo.CreateAsync(createdPurchasedPackage);
					var saver = await _unitOfWork.SaveAsync();
					await _unitOfWork.CommitTransactionAsync();
					if (!saver) return new BaseResponse { IsSuccess = false, Message = "Lưu gói đã mua của người dùng thất bại." };
					return new BaseResponse { IsSuccess = true, Message = "Lưu gói đã mua của người dùng thành công." };
				}

				return new BaseResponse { IsSuccess = false, Message = "Gói quảng cáo không tồn tại" };
			}
			catch (Exception)
			{
				await _unitOfWork.RollBackAsync();
				throw;
			}
		}
	}
}
