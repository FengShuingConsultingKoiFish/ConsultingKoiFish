using AutoMapper;
using ConsultingKoiFish.BLL.DTOs.AdvertisementPackageDTOs;
using ConsultingKoiFish.BLL.DTOs.BlogDTOs;
using ConsultingKoiFish.BLL.DTOs.PurchasedPackageDTOs;
using ConsultingKoiFish.BLL.DTOs.Response;
using ConsultingKoiFish.BLL.Helpers.Fillters;
using ConsultingKoiFish.BLL.Services.Interfaces;
using ConsultingKoiFish.DAL.Entities;
using ConsultingKoiFish.DAL.Enums;
using ConsultingKoiFish.DAL.Paging;
using ConsultingKoiFish.DAL.Queries;
using ConsultingKoiFish.DAL.UnitOfWork;
using Mailjet.Client.Resources;

namespace ConsultingKoiFish.BLL.Services.Implements
{
	public class PurchasedPackageService : IPurchasedPackageService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		private readonly IAdvertisementPackageService _advertisementPackageService;

		public PurchasedPackageService(IUnitOfWork unitOfWork, IMapper mapper, IAdvertisementPackageService advertisementPackageService)
		{
			this._unitOfWork = unitOfWork;
			this._mapper = mapper;
			this._advertisementPackageService = advertisementPackageService;
		}
		public async Task<BaseResponse> CreatePurchasedPacakge(PurchasedPackageCreateDTO dto)
		{
			try
			{
				await _unitOfWork.BeginTransactionAsync();
				var repo = _unitOfWork.GetRepo<PurchasedPackage>();
				var createdPurchasedPackage = _mapper.Map<PurchasedPackage>(dto);
				await repo.CreateAsync(createdPurchasedPackage);
				var saver = await _unitOfWork.SaveAsync();
				await _unitOfWork.CommitTransactionAsync();
				if (!saver) return new BaseResponse { IsSuccess = false, Message = "Lưu gói đã mua của người dùng thất bại." };
				return new BaseResponse { IsSuccess = true, Message = "Lưu gói đã mua của người dùng thành công." };
			}
			catch (Exception)
			{
				await _unitOfWork.RollBackAsync();
				throw;
			}
		}

		public async Task<BaseResponse> ExtendPurchasedPackage(PurchasedPackageViewDTO packageViewDTO, string userId)
		{
			try
			{
				await _unitOfWork.BeginTransactionAsync();
				var repo = _unitOfWork.GetRepo<PurchasedPackage>();

				var extendedPurchasedPackage = _mapper.Map<PurchasedPackage>(packageViewDTO);
				extendedPurchasedPackage.CreatedDate = DateTime.Now;
				extendedPurchasedPackage.Status = (int)PurchasedPackageStatus.Available;
				extendedPurchasedPackage.ExpireDate = DateTime.Now.AddDays(packageViewDTO.DurationsInDays);
				extendedPurchasedPackage.IsActive = true;

				await repo.UpdateAsync(extendedPurchasedPackage);
				var saver = await _unitOfWork.SaveAsync();
				await _unitOfWork.CommitTransactionAsync();
				if (!saver) return new BaseResponse { IsSuccess = false, Message = "Lưu gói đã mua của người dùng thất bại." };
				return new BaseResponse { IsSuccess = true, Message = "Lưu gói đã mua của người dùng thành công." };
			}
			catch (Exception)
			{
				await _unitOfWork.RollBackAsync();
				throw;
			}
		}

		public async Task<PaginatedList<PurchasedPackageViewDTO>> GetAllPurchasedPackageForUser(PurchasedPackageGetListDTO dto, string userId)
		{
			var repo = _unitOfWork.GetRepo<PurchasedPackage>();
			var loadedRecords = repo.Get(new QueryBuilder<PurchasedPackage>()
				.WithPredicate(x => x.IsActive == true && x.UserId.Equals(userId))
				.WithInclude(x => x.User)
				.WithTracking(false)
				.Build());

			if (dto.Status.HasValue)
			{
				loadedRecords = loadedRecords.Where(x => x.Status == (int)dto.Status);
			}

			if (dto.OrderDate.HasValue)
			{
				switch ((int)dto.OrderDate)
				{
					case 1: loadedRecords = loadedRecords.OrderByDescending(x => x.CreatedDate); break;
					case 2: loadedRecords = loadedRecords.OrderBy(x => x.CreatedDate); break;
				}
			}

			var pagedRecords = await PaginatedList<PurchasedPackage>.CreateAsync(loadedRecords, dto.PageIndex, dto.PageSize);
			var response = await ConvertPurchasedPackagesToPurchasedPackageViews(pagedRecords, dto.OrderImage);
			return new PaginatedList<PurchasedPackageViewDTO>(response, pagedRecords.TotalItems, dto.PageIndex, dto.PageSize);
		}


		public async Task<PurchasedPackageViewDTO> GetPurchasedPackageByIdForMember(int id, string userId, OrderImage? orderImage)
		{
			var repo = _unitOfWork.GetRepo<PurchasedPackage>();
			var package = await repo.GetSingleAsync(new QueryBuilder<PurchasedPackage>()
				.WithPredicate(x => x.Id == id && x.IsActive == true && x.UserId.Equals(userId))
				.WithTracking(false)
				.WithInclude(x => x.User)
				.Build());
			if (package == null) return null;
			var response = await ConvertPurchasedPackageToPurchasedPackageView(package, orderImage);
			return response;
		}

		public async Task<PurchasedPackageViewDTO> GetUnavailablePackageForMember(int purchasedPackagedId, string userId)
		{
			var repo = _unitOfWork.GetRepo<PurchasedPackage>();

			var extendedPurchasedPackage = await repo.GetSingleAsync(new QueryBuilder<PurchasedPackage>()
																	.WithPredicate(x => x.Id == purchasedPackagedId &&
																					x.UserId.Equals(userId) &&
																					 x.Status == (int)PurchasedPackageStatus.Unavailable &&
																					 x.IsActive == true)
																	.WithInclude(x => x.User)
																	.WithTracking(false)
																	.Build());
			if (extendedPurchasedPackage == null) return null;
			var response = await ConvertPurchasedPackageToPurchasedPackageView(extendedPurchasedPackage, OrderImage.DatetimeDescending);
			return response;
		}

		public async Task<PaginatedList<PurchasedPackageViewDTO>> GetAllPurchasedPackageForAdmin(PurchasedPackageGetListDTO dto)
		{
			var repo = _unitOfWork.GetRepo<PurchasedPackage>();
			var loadedRecords = repo.Get(new QueryBuilder<PurchasedPackage>()
				.WithPredicate(x => x.IsActive == true)
				.WithInclude(x => x.User)
				.WithTracking(false)
				.Build());

			if (dto.Status.HasValue)
			{
				loadedRecords = loadedRecords.Where(x => x.Status == (int)dto.Status);
			}

			if (dto.OrderDate.HasValue)
			{
				switch ((int)dto.OrderDate)
				{
					case 1: loadedRecords = loadedRecords.OrderByDescending(x => x.CreatedDate); break;
					case 2: loadedRecords = loadedRecords.OrderBy(x => x.CreatedDate); break;
				}
			}

			var pagedRecords = await PaginatedList<PurchasedPackage>.CreateAsync(loadedRecords, dto.PageIndex, dto.PageSize);
			var response = await ConvertPurchasedPackagesToPurchasedPackageViews(pagedRecords, dto.OrderImage);
			return new PaginatedList<PurchasedPackageViewDTO>(response, pagedRecords.TotalItems, dto.PageIndex, dto.PageSize);
		}

		public async Task<PurchasedPackageViewDTO> GetPurchasedPackageByIdForAdmin(int id, OrderImage? orderImage)
		{
			var repo = _unitOfWork.GetRepo<PurchasedPackage>();
			var package = await repo.GetSingleAsync(new QueryBuilder<PurchasedPackage>()
				.WithPredicate(x => x.Id == id && x.IsActive == true)
				.WithTracking(false)
				.WithInclude(x => x.User)
				.Build());
			if (package == null) return null;
			var response = await ConvertPurchasedPackageToPurchasedPackageView(package, orderImage);
			return response;
		}

		#region PRIVATE

		/// <summary>
		/// This is used to convert a collection of purchased package to purchasedPackageViewDTO
		/// </summary>
		/// <param name="purchasedPackages"></param>
		/// <param name="orderImage"></param>
		/// <returns></returns>
		private async Task<List<PurchasedPackageViewDTO>> ConvertPurchasedPackagesToPurchasedPackageViews(List<PurchasedPackage> purchasedPackages, OrderImage? orderImage)
		{
			var response = new List<PurchasedPackageViewDTO>();
			foreach (var package in purchasedPackages)
			{
				var childResponse = new PurchasedPackageViewDTO(package);
				response.Add(childResponse);
			}
			return response;
		}


		/// <summary>
		/// This is used to convert a purchased package to purchasedPackageViewDTO
		/// </summary>
		/// <param name="package"></param>
		/// <param name="orderImage"></param>
		/// <returns></returns>
		public async Task<PurchasedPackageViewDTO> ConvertPurchasedPackageToPurchasedPackageView(PurchasedPackage package, OrderImage? orderImage)
		{
			var response = new PurchasedPackageViewDTO(package);
			return response;
		}

		#endregion
	}
}
