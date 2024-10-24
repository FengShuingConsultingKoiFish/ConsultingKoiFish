using AutoMapper;
using ConsultingKoiFish.BLL.DTOs.AdImageDTOs;
using ConsultingKoiFish.BLL.DTOs.AdvertisementDTOs;
using ConsultingKoiFish.BLL.DTOs.BlogDTOs;
using ConsultingKoiFish.BLL.DTOs.BlogImageDTOs;
using ConsultingKoiFish.BLL.DTOs.CommentDTOs;
using ConsultingKoiFish.BLL.DTOs.ImageDTOs;
using ConsultingKoiFish.BLL.DTOs.Response;
using ConsultingKoiFish.BLL.Services.Interfaces;
using ConsultingKoiFish.DAL.Entities;
using ConsultingKoiFish.DAL.Enums;
using ConsultingKoiFish.DAL.Paging;
using ConsultingKoiFish.DAL.Queries;
using ConsultingKoiFish.DAL.UnitOfWork;
using Mailjet.Client.Resources;
using Microsoft.EntityFrameworkCore;
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
		private readonly IImageService _imageService;
		private readonly ICommentService _commentService;

		public AdvertisementService(IUnitOfWork unitOfWork, IMapper mapper, IImageService imageService, ICommentService commentService)
		{
			this._unitOfWork = unitOfWork;
			this._mapper = mapper;
			this._imageService = imageService;
			this._commentService = commentService;
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
					if (selectedPurchasedPackage == null || selectedPurchasedPackage.Status == (int)PurchasedPackageStatus.Unavailable)
						return new BaseResponse { IsSuccess = false, Message = "Gói bạn chọn không khả dụng." };

					if (dto.Description.Length > selectedPurchasedPackage.AdvertisementPackage.LimitContent)
						return new BaseResponse { IsSuccess = false, Message = "Số kí tự trong phần mô tả đã vượt quá giới hạn cho phép của gói quảng cáo." };

					//<===Thực hiện việc tạo mới ad===>
					var createdAdDto = new AdvertisementCreateDTO(dto, userId);
					var createdAd = _mapper.Map<Advertisement>(createdAdDto);
					await repo.CreateAsync(createdAd);
					await _unitOfWork.SaveChangesAsync();

					//<===Kiểm trá và cập nhật số lượng sau khi create===>
					selectedPurchasedPackage.MornitoredQuantity += 1;
					if (selectedPurchasedPackage.MornitoredQuantity >= selectedPurchasedPackage.AdvertisementPackage.LimitAd)
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

				if (advertisement.AdImages.Count() + dto.ImagesId.Count() > adPackage.LimitImage)
				{
					var availableImages = adPackage.LimitImage - advertisement.AdImages.Count();
					return new BaseResponse { IsSuccess = false, Message = $"Bạn chỉ có thể thêm {availableImages} ảnh vào quảng cáo này nữa." };
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
					foreach (var imageId in dto.ImageIds)
					{
						var deleteAdImage = await repo.GetSingleAsync(new QueryBuilder<AdImage>()
							.WithPredicate(x => x.ImageId == imageId && x.AdvertisementId == dto.AdvertisementId)
							.WithTracking(false)
							.Build());
						if (deleteAdImage == null)
							return new BaseResponse
							{ IsSuccess = false, Message = $"Ảnh {imageId} không tồn tại trong Blog" };
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

		public async Task<PaginatedList<AdvertisementViewDTO>> GetAllAdvertisements(AdvertisementGetListDTO dto)
		{
			var repo = _unitOfWork.GetRepo<Advertisement>();
			var imageRepo = _unitOfWork.GetRepo<Advertisement>();
			var loadedRecords = repo.Get(new QueryBuilder<Advertisement>()
				.WithPredicate(x => x.IsActive == true && x.Status == (int)AdvertisementStatus.Approved)
				.WithTracking(false)
				.WithInclude(x => x.User)
				.Build());
			if (dto.Title != null)
			{
				loadedRecords = loadedRecords.Where(x => x.Title.Contains(dto.Title));
			}

			if (dto.OrderAdvertisement.HasValue)
			{
				switch ((int)dto.OrderAdvertisement)
				{
					case 1: loadedRecords = loadedRecords.OrderByDescending(x => x.CreatedDate); break;
					case 2: loadedRecords = loadedRecords.OrderBy(x => x.CreatedDate); break;
				}
			}
			var pagedRecords = await PaginatedList<Advertisement>.CreateAsync(loadedRecords, dto.PageIndex, dto.PageSize);
			var response = await ConvertAdsToAdViews(pagedRecords, dto.OrderComment, dto.OrderImage);
			return new PaginatedList<AdvertisementViewDTO>(response, pagedRecords.TotalItems, dto.PageIndex, dto.PageSize);
		}


		public async Task<AdvertisementViewDTO> GetAdvertisementById(int id, OrderComment? orderComment, OrderImage? orderImage)
		{
			var repo = _unitOfWork.GetRepo<Advertisement>();
			var advertisement = await repo.GetSingleAsync(new QueryBuilder<Advertisement>()
				.WithPredicate(x => x.Id == id && x.IsActive == true)
				.WithTracking(false)
				.WithInclude(x => x.User)
				.Build());
			if (advertisement == null) return null;
			var response = await ConvertAdToAdView(advertisement, orderComment, orderImage);
			return response;
		}

		public async Task<PaginatedList<AdvertisementViewDTO>> GetAllAdvertisementsForUser(string userId, AdvertisementGetListDTO dto)
		{
			var repo = _unitOfWork.GetRepo<Advertisement>();
			var imageRepo = _unitOfWork.GetRepo<Image>();
			var loadedRecords = repo.Get(new QueryBuilder<Advertisement>()
				.WithPredicate(x => x.IsActive == true && x.UserId.Equals(userId))
				.WithTracking(false)
				.WithInclude(x => x.User)
				.Build());

			if (dto.Title != null)
			{
				loadedRecords = loadedRecords.Where(x => x.Title.Contains(dto.Title));
			}

			if (dto.AdvertisementStatus.HasValue)
			{
				loadedRecords = loadedRecords.Where(x => x.Status == (int)dto.AdvertisementStatus);
			}

			if (dto.OrderAdvertisement.HasValue)
			{
				switch ((int)dto.OrderAdvertisement)
				{
					case 1: loadedRecords = loadedRecords.OrderByDescending(x => x.CreatedDate); break;
					case 2: loadedRecords = loadedRecords.OrderBy(x => x.CreatedDate); break;
				}
			}
			var pagedRecords = await PaginatedList<Advertisement>.CreateAsync(loadedRecords, dto.PageIndex, dto.PageSize);
			var response = await ConvertAdsToAdViews(pagedRecords, dto.OrderComment, dto.OrderImage);
			return new PaginatedList<AdvertisementViewDTO>(response, pagedRecords.TotalItems, dto.PageIndex, dto.PageSize);
		}

		public async Task<PaginatedList<AdvertisementViewDTO>> GetAllBlogsForAdmin(AdvertisementGetListDTO dto)
		{
			var repo = _unitOfWork.GetRepo<Advertisement>();
			var imageRepo = _unitOfWork.GetRepo<Image>();
			var loadedRecords = repo.Get(new QueryBuilder<Advertisement>()
				.WithPredicate(x => x.IsActive == true)
				.WithTracking(false)
				.WithInclude(x => x.User)
				.Build());

			if (dto.Title != null)
			{
				loadedRecords = loadedRecords.Where(x => x.Title.Contains(dto.Title));
			}

			if (dto.AdvertisementStatus.HasValue)
			{
				loadedRecords = loadedRecords.Where(x => x.Status == (int)dto.AdvertisementStatus);
			}

			if (dto.OrderAdvertisement.HasValue)
			{
				switch ((int)dto.OrderAdvertisement)
				{
					case 1: loadedRecords = loadedRecords.OrderByDescending(x => x.CreatedDate); break;
					case 2: loadedRecords = loadedRecords.OrderBy(x => x.CreatedDate); break;
				}
			}
			var pagedRecords = await PaginatedList<Advertisement>.CreateAsync(loadedRecords, dto.PageIndex, dto.PageSize);
			var response = await ConvertAdsToAdViews(pagedRecords, dto.OrderComment, dto.OrderImage);
			return new PaginatedList<AdvertisementViewDTO>(response, pagedRecords.TotalItems, dto.PageIndex, dto.PageSize);
		}

		public async Task<BaseResponse> UpdateStatusAdvertisement(AdvertisementUpdateStatusDTO dto)
		{
			try
			{
				await _unitOfWork.BeginTransactionAsync();
				var repo = _unitOfWork.GetRepo<Advertisement>();
				var any = await repo.AnyAsync(new QueryBuilder<Advertisement>()
					.WithPredicate(x => x.Id == dto.Id)
					.Build());
				if (any)
				{
					var advertisement = await repo.GetSingleAsync(new QueryBuilder<Advertisement>()
						.WithPredicate(x => x.Id == dto.Id)
						.WithTracking(false)
						.WithInclude(x => x.User, x => x.PurchasedPackage)
						.Build());

					if ((int)dto.Status == 3)
					{
						advertisement.PurchasedPackage.MornitoredQuantity -= 1;
						if (advertisement.PurchasedPackage.Status == (int)PurchasedPackageStatus.Unavailable)
							advertisement.PurchasedPackage.Status = (int)PurchasedPackageStatus.Available;
						await _unitOfWork.GetRepo<PurchasedPackage>().UpdateAsync(advertisement.PurchasedPackage);
						await _unitOfWork.SaveChangesAsync();
					}
					advertisement.Status = (int)dto.Status;
					await repo.UpdateAsync(advertisement);
					await _unitOfWork.CommitTransactionAsync();
					var saver = await _unitOfWork.SaveAsync();
					if (!saver)
					{
						return new BaseResponse { IsSuccess = false, Message = "Lưu dữ liệu không thành công." };
					}
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

		#region PRIVATE

		/// <summary>
		/// This is used to get ad images for each Advertisement
		/// </summary>
		/// <param name="advertisement"></param>
		/// <param name="orderImage"></param>
		/// <returns></returns>
		private async Task<List<AdImage>> GetAdImagesForEachAdvertisement(Advertisement advertisement, OrderImage? orderImage)
		{
			var adImageRepo = _unitOfWork.GetRepo<AdImage>();
			var adImages = adImageRepo.Get(new QueryBuilder<AdImage>()
													.WithPredicate(x => x.AdvertisementId == advertisement.Id)
													.WithTracking(false)
													.Build());
			if (orderImage.HasValue)
			{
				switch ((int)orderImage)
				{
					case 1: adImages = adImages.OrderByDescending(x => x.Id); break;
					case 2: adImages = adImages.OrderBy(x => x.Id); break;
				}
			}
			return await adImages.ToListAsync();
		}

		/// <summary>
		/// This is used to convert a collection of ad images to a collection of imageViewDTOs
		/// </summary>
		/// <param name="adImages"></param>
		/// <returns></returns>
		private Task<List<ImageViewDTO>> ConvertAdImagesToImageViews(ICollection<AdImage> adImages)
		{
			return _imageService.ConvertSpeciedImageToImageViews(adImages, adImage => adImage.ImageId);
		}


		/// <summary>
		/// This is used to get an collection of AdComment for each Ad
		/// </summary>
		/// <param name="advertisement"></param>
		/// <param name="orderComment"></param>
		/// <returns></returns>
		private async Task<List<AdComment>> GetAdCommentsForEachAdvertisement(Advertisement advertisement, OrderComment? orderComment)
		{
			var adCommentRepo = _unitOfWork.GetRepo<AdComment>();
			var adComments = adCommentRepo.Get(new QueryBuilder<AdComment>()
													.WithPredicate(x => x.AdvertisementId == advertisement.Id)
													.WithTracking(false)
													.Build());
			if (orderComment.HasValue)
			{
				switch ((int)orderComment)
				{
					case 1: adComments = adComments.OrderByDescending(x => x.Id); break;
					case 2: adComments = adComments.OrderBy(x => x.Id); break;
				}
			}
			return await adComments.ToListAsync();
		}


		/// <summary>
		/// this is used to convert ad comments to commentViewDTOs
		/// </summary>
		/// <param name="adComments"></param>
		/// <returns></returns>
		private Task<List<CommentViewDTO>> ConvertAdCommentsToCommentViews(ICollection<AdComment> adComments)
		{
			var commentViews = _commentService.ConvertSpeciedCommentToCommentViews(adComments, adComment => adComment.CommentId);
			return commentViews;
		}


		/// <summary>
		/// This is used to convert a collection of ads to a collection of adViewDtos
		/// </summary>
		/// <param name="ads"></param>
		/// <param name="orderComment"></param>
		/// <param name="orderImage"></param>
		/// <returns></returns>
		private async Task<List<AdvertisementViewDTO>> ConvertAdsToAdViews(List<Advertisement> ads, OrderComment? orderComment, OrderImage? orderImage)
		{
			var response = new List<AdvertisementViewDTO>();
			foreach (var ad in ads)
			{
				var adImageViewDtos = await ConvertAdImagesToImageViews(await GetAdImagesForEachAdvertisement(ad, orderImage));
				var adCommentViewDtos = await ConvertAdCommentsToCommentViews(await GetAdCommentsForEachAdvertisement(ad, orderComment));
				var childResponse = new AdvertisementViewDTO(ad, adImageViewDtos, adCommentViewDtos);
				response.Add(childResponse);
			}
			return response;
		}


		/// <summary>
		/// This is used to convert an ad to adViewDto
		/// </summary>
		/// <param name="ad"></param>
		/// <param name="orderComment"></param>
		/// <param name="orderImage"></param>
		/// <returns></returns>
		private async Task<AdvertisementViewDTO> ConvertAdToAdView(Advertisement ad, OrderComment? orderComment, OrderImage? orderImage)
		{
			var adImageViewDtos = await ConvertAdImagesToImageViews(await GetAdImagesForEachAdvertisement(ad, orderImage));
			var adCommentViewDtos = await ConvertAdCommentsToCommentViews(await GetAdCommentsForEachAdvertisement(ad, orderComment));
			var response = new AdvertisementViewDTO(ad, adImageViewDtos, adCommentViewDtos);
			return response;
		}

		#endregion
	}
}
