using AutoMapper;
using ConsultingKoiFish.BLL.DTOs.AdImageDTOs;
using ConsultingKoiFish.BLL.DTOs.AdvertisementDTOs;
using ConsultingKoiFish.BLL.DTOs.BlogDTOs;
using ConsultingKoiFish.BLL.DTOs.BlogImageDTOs;
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
	public class AdvertisementService : IAdvertisementService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public AdvertisementService(IUnitOfWork unitOfWork, IMapper mapper)
        {
			this._unitOfWork = unitOfWork;
			this._mapper = mapper;
		}

		public async Task<BaseResponse> CreateUpdateAdvertisement(AdvertisementRequestDTO dto, string userId)
		{
			try
			{
				await _unitOfWork.BeginTransactionAsync();
				var repo = _unitOfWork.GetRepo<Advertisement>();
				var adImageRepo = _unitOfWork.GetRepo<AdImage>();
				var purchasedPacakgeRepo = _unitOfWork.GetRepo<PurchasedPackage>();
				var any = await repo.AnyAsync(new QueryBuilder<Advertisement>()
					.WithPredicate(x => x.Id == dto.Id)
					.Build());
				if (any)
				{
					var advertisement = await repo.GetSingleAsync(new QueryBuilder<Advertisement>()
						.WithPredicate(x => x.Id == dto.Id)
						.WithTracking(false)
						.Build());
					if (!advertisement.UserId.Equals(userId))
						return new BaseResponse
						{
							IsSuccess = false,
							Message = "Quảng cáo không thuộc sở hữu người dùng."
						};
					var updateAdDto = new AdvertisementUpdateDTO(dto);
					var updateAd = _mapper.Map(updateAdDto, advertisement);
					await repo.UpdateAsync(updateAd);
				}
				else
				{
					var selectedPurchasedPackage = await purchasedPacakgeRepo.GetSingleAsync(new QueryBuilder<PurchasedPackage>()
																							.WithPredicate(x => x.Id == dto.PurchasedPackageId && x.IsActive == true)
																							.WithInclude(x => x.AdvertisementPackage)
																							.WithTracking(false)
																							.Build());
					//<===Check valid của gói===>
					if(selectedPurchasedPackage == null || selectedPurchasedPackage.Status == (int)PurchasedPackageStatus.Unavailable)
						return new BaseResponse { IsSuccess = false, Message = "Gói bạn chọn không khả dụng."};

					if(dto.Description.Length > selectedPurchasedPackage.AdvertisementPackage.LimitContent)
						return new BaseResponse { IsSuccess = false, Message = "Số kí tự trong phần mô tả đã vượt quá giới hạn cho phép của gói quảng cáo."};

					//<===Thực hiện việc tạo mới ad===>
					var createdAdDto = new AdvertisementCreateDTO(dto, userId);
					var createdAd = _mapper.Map<Advertisement>(createdAdDto);
					await repo.CreateAsync(createdAd);
					await _unitOfWork.SaveChangesAsync();

					//<===Kiểm trá và cập nhật số lượng sau khi create===>
					selectedPurchasedPackage.MornitoredQuantity += 1;
					if(selectedPurchasedPackage.MornitoredQuantity >= selectedPurchasedPackage.AdvertisementPackage.LimitAd)
						selectedPurchasedPackage.Status = (int)PurchasedPackageStatus.Unavailable;
					await purchasedPacakgeRepo.UpdateAsync(selectedPurchasedPackage);
					await _unitOfWork.SaveChangesAsync();



					if (dto.ImageIds != null || dto.ImageIds.Any())
					{
						if (dto.ImageIds.Count > selectedPurchasedPackage.AdvertisementPackage.LimitImage)
							return new BaseResponse { IsSuccess = false, Message = "Số ảnh trong quảng cáo đã vượt quá giới hạn cho phép của gói." };

						var createdAdImageDTOs = new List<AdImageCreateDTO>();
						foreach (var image in dto.ImageIds)
						{
							var existedImage = await _unitOfWork.GetRepo<Image>().GetSingleAsync(new QueryBuilder<Image>()
																								.WithPredicate(x => x.Id == image && x.IsActive == true)
																								.WithTracking(false)
																								.Build());
							if (existedImage == null) return new BaseResponse { IsSuccess = false, Message = $"Ảnh {image} không tồn tại." };
							var createdAdImageDto = new AdImageCreateDTO
							{
								AdvertisementId = createdAd.Id,
								ImageId = image
							};
							createdAdImageDTOs.Add(createdAdImageDto);
						}
						var createdAdImages = _mapper.Map<List<AdImage>>(createdAdImageDTOs);
						await adImageRepo.CreateAllAsync(createdAdImages);
					}
				}

				var saver = await _unitOfWork.SaveAsync();
				await _unitOfWork.CommitTransactionAsync();
				if (!saver)
				{
					return new BaseResponse
					{
						IsSuccess = false,
						Message = "Lưu dữ liệu không thành công."
					};
				}

				return new BaseResponse { IsSuccess = true, Message = "Lưu dữ liệu thành công." };
			}
			catch (Exception)
			{
				await _unitOfWork.RollBackAsync();
				throw;
			}
		}

		public async Task<BaseResponse> AddImagesToAdvertisement(AdImageRequestDTO dto)
		{
			try
			{
				await _unitOfWork.BeginTransactionAsync();
				var repo = _unitOfWork.GetRepo<AdImage>();
				var adRepo = _unitOfWork.GetRepo<Advertisement>();
				var adPackageRepo = _unitOfWork.GetRepo<AdvertisementPackage>();
				var advertisement = await adRepo.GetSingleAsync(new QueryBuilder<Advertisement>()
														.WithPredicate(x => x.Id == dto.AdvertisementId && x.IsActive == true)
														.WithInclude(x => x.PurchasedPackage, x => x.AdImages)
														.WithTracking(false)
														.Build());
				var adPackage = await adPackageRepo.GetSingleAsync(new QueryBuilder<AdvertisementPackage>()
																.WithPredicate(x => x.Id == advertisement.PurchasedPackage.AdvertisementPackageId)
																.WithTracking(false)
																.Build());

				if(advertisement.AdImages.Count() + dto.ImagesId.Count() > adPackage.LimitImage)
				{
					var availableImages = adPackage.LimitImage - advertisement.AdImages.Count();
					return new BaseResponse { IsSuccess = false, Message = $"Bạn chỉ có thể thêm {availableImages} ảnh vào quảng cáo này nữa."};
				}

				var createdAdImageDTOs = new List<AdImageCreateDTO>();
				foreach (var image in dto.ImagesId)
				{
					var any = await repo.AnyAsync(new QueryBuilder<AdImage>()
						.WithPredicate(x => x.ImageId == image && x.AdvertisementId == dto.AdvertisementId)
						.WithTracking(false)
						.Build());

					if (any) return new BaseResponse { IsSuccess = false, Message = $"Ảnh {image} đã tổn tại trong quảng cáo." };
					var createdAdImageDTO = new AdImageCreateDTO()
					{
						AdvertisementId = dto.AdvertisementId,
						ImageId = image
					};
					createdAdImageDTOs.Add(createdAdImageDTO);
				}
				var adImages = _mapper.Map<List<AdImage>>(createdAdImageDTOs);
				await repo.CreateAllAsync(adImages);
				var saver = await _unitOfWork.SaveAsync();
				await _unitOfWork.CommitTransactionAsync();
				if (!saver) return new BaseResponse { IsSuccess = false, Message = "Lưu thất bại" };
				return new BaseResponse { IsSuccess = true, Message = "Lưu thành công." };
			}
			catch (Exception)
			{
				await _unitOfWork.RollBackAsync();
				throw;
			}
		}

		public async Task<BaseResponse> DeleteImagesFromAdvertisement(AdImageDeleteDTO dto)
		{
			try
			{
				await _unitOfWork.BeginTransactionAsync();
				var repo = _unitOfWork.GetRepo<AdImage>();
				var adRepo = _unitOfWork.GetRepo<Advertisement>();
				var deletedAdImages = new List<AdImage>();
				var any = await adRepo.AnyAsync(new QueryBuilder<Advertisement>()
					.WithPredicate(x => x.Id == dto.AdvertisementId)
					.WithTracking(false)
					.Build());
				if (any)
				{
					foreach (var adImageId in dto.AdImageIds)
					{
						var deleteAdImage = await repo.GetSingleAsync(new QueryBuilder<AdImage>()
							.WithPredicate(x => x.Id == adImageId && x.AdvertisementId == dto.AdvertisementId)
							.WithTracking(false)
							.Build());
						if (deleteAdImage == null)
							return new BaseResponse
							{ IsSuccess = false, Message = $"Ảnh {adImageId} không tồn tại trong Blog" };
						deletedAdImages.Add(deleteAdImage);
					}

					await repo.DeleteAllAsync(deletedAdImages);
					var saver = await _unitOfWork.SaveAsync();
					await _unitOfWork.CommitTransactionAsync();
					if (!saver) return new BaseResponse { IsSuccess = false, Message = "Lưu dữ liệu thất bại" };
					return new BaseResponse { IsSuccess = true, Message = "Lưu dữ liệu thành công." };
				}
				return new BaseResponse { IsSuccess = false, Message = "Quảng cáo không tồn tại." };
			}
			catch (Exception)
			{
				await _unitOfWork.RollBackAsync();
				throw;
			}
		}
	}
}
