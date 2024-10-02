using AutoMapper;
using ConsultingKoiFish.BLL.DTOs.ImageDTOs;
using ConsultingKoiFish.BLL.DTOs.Response;
using ConsultingKoiFish.BLL.Services.Interfaces;
using ConsultingKoiFish.DAL.Entities;
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
					var createdImageDto = new ImageCreateDTO { AltText = requestDTO.AltText, FilePath = requestDTO.FilePath };
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
			var response = _mapper.Map<ImageViewDTO>(image);
			response.UserName = image.User.UserName;
			response.CreatedDate = image.CreatedDate.ToString("dd/MM/yyyy");
			return response;
		}

		public async Task<PaginatedList<ImageViewDTO>> GetListImageByName(string? name, string userId, int pageIndex, int pageSize)
		{
			var repo = _unitOfWork.GetRepo<Image>();
			var loadedRecords = repo.Get(new QueryBuilder<Image>()
												.WithPredicate(x => x.UserId.Equals(userId) && x.IsActive == true)
												.WithTracking(false)
												.WithInclude(x => x.User)
												.Build());
			if (!string.IsNullOrEmpty(name))
			{
				loadedRecords = loadedRecords.Where(x => x.AltText.Contains(name));
			}

			var pagedRecords = await PaginatedList<Image>.CreateAsync(loadedRecords, pageIndex, pageSize);
			var response = new List<ImageViewDTO>();
			foreach (var image in pagedRecords)
			{
				var childResponse = _mapper.Map<ImageViewDTO>(image);
				childResponse.UserName = image.User.UserName;
				childResponse.CreatedDate = image.CreatedDate.ToString("dd/MM/yyyy");
				response.Add(childResponse);
			}
			return new PaginatedList<ImageViewDTO>(response, pagedRecords.TotalItems, pageIndex, pageSize);
		}

		public async Task<PaginatedList<ImageViewDTO>> GetListImageByUserId(string userId, int pageIndex, int pageSize)
		{
			var repo = _unitOfWork.GetRepo<Image>();
			var loadedRecords = repo.Get(new QueryBuilder<Image>()
												.WithPredicate(x => x.UserId.Equals(userId) && x.IsActive == true)
												.WithTracking(false)
												.WithInclude(x => x.User)
												.Build());
			var pagedRecords = await PaginatedList<Image>.CreateAsync(loadedRecords, pageIndex, pageSize);
			var response = new List<ImageViewDTO>();
			foreach (var image in pagedRecords)
			{
				var childResponse = _mapper.Map<ImageViewDTO>(image);
				childResponse.UserName = image.User.UserName;
				childResponse.CreatedDate = image.CreatedDate.ToString("dd/MM/yyyy");
				response.Add(childResponse);
			}
			return new PaginatedList<ImageViewDTO>(response, pagedRecords.TotalItems, pageIndex, pageSize);
		}
	}
}

