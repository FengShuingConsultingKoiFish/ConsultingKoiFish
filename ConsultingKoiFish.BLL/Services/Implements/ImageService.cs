using AutoMapper;
using ConsultingKoiFish.BLL.DTOs.ImageDTOs;
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
				if(any)
				{
					var image = await repo.GetSingleAsync(new QueryBuilder<Image>()
														.WithPredicate(x => x.Id == requestDTO.Id)
														.Build());
					if(!image.UserId.Equals(userId)) return new BaseResponse { IsSuccess = false, Message = "Ảnh không khớp với người dùng."};
					var updatedImageDto = new ImageUpdateDTO { AltText = requestDTO.AltText };
					var updatedImage = _mapper.Map(updatedImageDto, image);
					await repo.UpdateAsync(updatedImage);
				}
				else
				{
					var createdImageDto = new ImageCreateDTO { AltText= requestDTO.AltText, FilePath = requestDTO.FilePath};
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
	}
}
