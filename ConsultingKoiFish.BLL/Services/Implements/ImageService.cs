﻿using AutoMapper;
using ConsultingKoiFish.BLL.DTOs.ImageDTOs;
using ConsultingKoiFish.BLL.DTOs.Response;
using ConsultingKoiFish.BLL.Services.Interfaces;
using ConsultingKoiFish.DAL.Entities;
using ConsultingKoiFish.DAL.Paging;
using ConsultingKoiFish.DAL.Queries;
using ConsultingKoiFish.DAL.UnitOfWork;
using Mailjet.Client.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.BLL.Services.Implements
{
	public class ImageService : IImageService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public ImageService(IUnitOfWork unitOfWork, IMapper mapper)
		{
			this._unitOfWork = unitOfWork;
			this._mapper = mapper;
		}

		public async Task<BaseResponse> CreateUpdateImage(ImageRequestDTO requestDTO, string userId)
		{
			try
			{
				await _unitOfWork.BeginTransactionAsync();
				var repo = _unitOfWork.GetRepo<Image>();

				var any = await repo.AnyAsync(new QueryBuilder<Image>()
										.WithPredicate(x => x.Id == requestDTO.Id)
										.Build());
				if (any)
				{
					var image = await repo.GetSingleAsync(new QueryBuilder<Image>()
														.WithPredicate(x => x.Id == requestDTO.Id)
														.Build());
					if (!image.UserId.Equals(userId)) return new BaseResponse { IsSuccess = false, Message = "Ảnh không khớp với người dùng." };
					var updatedImageDto = new ImageUpdateDTO { AltText = requestDTO.AltText };
					var updatedImage = _mapper.Map(updatedImageDto, image);
					await repo.UpdateAsync(updatedImage);
				}
				else
				{
					var createdImageDto = new ImageCreateDTO { AltText = requestDTO.AltText, FilePath = requestDTO.Url };
					var createdImage = _mapper.Map<Image>(createdImageDto);
					createdImage.UserId = userId;
					createdImage.IsActive = true;
					createdImage.CreatedDate = DateTime.Now;
					await repo.CreateAsync(createdImage);
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

		public async Task<ImageViewDTO> CreateImage(ImageRequestDTO requestDTO, string userId)
		{
			try
			{
				await _unitOfWork.BeginTransactionAsync();
				var repo = _unitOfWork.GetRepo<Image>();
				var createdImageDto = new ImageCreateDTO { AltText = requestDTO.AltText, FilePath = requestDTO.Url };
				var createdImage = _mapper.Map<Image>(createdImageDto);
				createdImage.UserId = userId;
				createdImage.IsActive = true;
				createdImage.CreatedDate = DateTime.Now;
				await repo.CreateAsync(createdImage);
				var saver = await _unitOfWork.SaveAsync();
				await _unitOfWork.CommitTransactionAsync();
				if (!saver)
				{
					return null;
				}

				var response = await GetImageById(createdImage.Id);
				return response;
			}
			catch (Exception)
			{
				await _unitOfWork.RollBackAsync();
				throw;
			}
		}
		
		public async Task<BaseResponse> DeleteImage(int id, string userId)
		{
			var repo = _unitOfWork.GetRepo<Image>();
			var any = await repo.AnyAsync(new QueryBuilder<Image>()
											.WithPredicate(x => x.Id == id)
											.Build());
			if (any)
			{
				var image = await repo.GetSingleAsync(new QueryBuilder<Image>()
														.WithPredicate(x => x.Id == id)
														.WithTracking(false)
														.WithInclude(x => x.User)
														.Build());
				if (!image.UserId.Equals(userId)) return new BaseResponse { IsSuccess = false, Message = "Ảnh không khớp với người dùng." };
				image.IsActive = false;
				await repo.UpdateAsync(image);
				var saver = await _unitOfWork.SaveAsync();
				if (!saver)
				{
					return new BaseResponse { IsSuccess = false, Message = "Lưu dữ liệu không thành công." };
				}
				return new BaseResponse { IsSuccess = true, Message = "Lưu dữ liệu thành công." };
			}
			return new BaseResponse { IsSuccess = false, Message = "Ảnh không tồn tại." };
		}

		public async Task<ImageViewDTO> GetImageById(int id)
		{
			var repo = _unitOfWork.GetRepo<Image>();
			var image = await repo.GetSingleAsync(new QueryBuilder<Image>()
													.WithPredicate(x => x.Id == id && x.IsActive == true)
													.WithTracking(false)
													.WithInclude(x => x.User)
													.Build());
			if (image == null) return null;
			var response = new ImageViewDTO(image);
			return response;
		}

		
		public async Task<PaginatedList<ImageViewDTO>> GetListImageForMember(ImageGetListDTO dto, string userId)
		{
			var repo = _unitOfWork.GetRepo<Image>();
			var loadedRecords = repo.Get(new QueryBuilder<Image>()
												.WithPredicate(x => x.UserId.Equals(userId) && x.IsActive == true)
												.WithTracking(false)
												.WithInclude(x => x.User)
												.Build());
			if(!string.IsNullOrEmpty(dto.Name))
			{
				loadedRecords = loadedRecords.Where(x => x.AltText.Contains(dto.Name));
			}

			if(dto.OrderDate.HasValue)
			{
				switch((int)dto.OrderDate)
				{
					case 1: loadedRecords = loadedRecords.OrderByDescending(x => x.CreatedDate); break;
					case 2: loadedRecords = loadedRecords.OrderBy(x => x.CreatedDate); break;
				}
			}
			var pagedRecords = await PaginatedList<Image>.CreateAsync(loadedRecords, dto.PageIndex, dto.PageSize);
			var response = ConvertToImageViews(pagedRecords);
			return new PaginatedList<ImageViewDTO>(response, pagedRecords.TotalItems, dto.PageIndex, dto.PageSize);
		}

		#region Convert
		/// <summary>
		/// This is used to convert a collection of any image type (BlogImage, PackageImage, etc.) 
		/// to a collection of ImageViewDTOs.
		/// </summary>
		/// <typeparam name="TImage">The type of image collection (e.g., BlogImage, PackageImage)</typeparam>
		/// <param name="images">The collection of images</param>
		/// <param name="getImageId">A function to extract the ImageId from each image object</param>
		/// <returns>A list of ImageViewDTOs</returns>
		public async Task<List<ImageViewDTO>> ConvertSpeciedImageToImageViews<TImage>(ICollection<TImage> images, Func<TImage, int> getImageId)
		{
			var imageRepo = _unitOfWork.GetRepo<Image>();
			var responseImages = new List<ImageViewDTO>();

			foreach (var imageItem in images)
			{
				var imageId = getImageId(imageItem);
				var image = await imageRepo.GetSingleAsync(new QueryBuilder<Image>()
					.WithPredicate(x => x.Id == imageId)
					.WithInclude(x => x.User)
					.WithTracking(false)
					.Build());

				var childResponseImage = new ImageViewDTO(image);
				responseImages.Add(childResponseImage);
			}

			return responseImages;
		}

		/// <summary>
		/// This is used to convert a collection of Image to a collection of imageViewDTOs
		/// </summary>
		/// <param name="images"></param>
		/// <returns></returns>
		public List<ImageViewDTO> ConvertToImageViews(ICollection<Image> images)
		{
			var imageRepo = _unitOfWork.GetRepo<Image>();
			var responseImages = new List<ImageViewDTO>();

			foreach (var imageItem in images)
			{
				var childResponseImage = new ImageViewDTO(imageItem);
				responseImages.Add(childResponseImage);
			}

			return responseImages;
		}

		#endregion

		
	}
}

