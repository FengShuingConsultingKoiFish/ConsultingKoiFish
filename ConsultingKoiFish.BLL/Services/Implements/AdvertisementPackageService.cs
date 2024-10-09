using AutoMapper;
using ConsultingKoiFish.BLL.DTOs.AdvertisementPackageDTOs;
using ConsultingKoiFish.BLL.DTOs.BlogDTOs;
using ConsultingKoiFish.BLL.DTOs.BlogImageDTOs;
using ConsultingKoiFish.BLL.DTOs.PackageImageDTOs;
using ConsultingKoiFish.BLL.DTOs.Response;
using ConsultingKoiFish.BLL.Services.Interfaces;
using ConsultingKoiFish.DAL.Entities;
using ConsultingKoiFish.DAL.Enums;
using ConsultingKoiFish.DAL.Queries;
using ConsultingKoiFish.DAL.UnitOfWork;

namespace ConsultingKoiFish.BLL.Services.Implements;

public class AdvertisementPackageService : IAdvertisementPackageService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AdvertisementPackageService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

	public async Task<BaseResponse> AddImagesToPackage(PackageImageRequestDTO dto)
	{
		try
		{
			await _unitOfWork.BeginTransactionAsync();
			var repo = _unitOfWork.GetRepo<PackageImage>();
			var createdPackageImageDTOs = new List<PackageImageCreateDTO>();
			foreach (var image in dto.ImagesId)
			{
				var any = await repo.AnyAsync(new QueryBuilder<PackageImage>()
					.WithPredicate(x => x.ImageId == image && x.AdvertisementPackageId == dto.AdvertisementPackageId)
					.WithTracking(false)
					.Build());

				if (any) return new BaseResponse { IsSuccess = false, Message = $"Ảnh {image} đã tổn tại trong gói." };
				var createdPackageImageDTO = new PackageImageCreateDTO()
				{
					AdvertisementPackageId = dto.AdvertisementPackageId,
					ImageId = image
				};
				createdPackageImageDTOs.Add(createdPackageImageDTO);
			}
			var packageImages = _mapper.Map<List<PackageImage>>(createdPackageImageDTOs);
			await repo.CreateAllAsync(packageImages);
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

	public async Task<BaseResponse> CreateUpdateAdvertisementPackage(AdvertisementPackageRequestDTO dto, string userName)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            var repo = _unitOfWork.GetRepo<AdvertisementPackage>();
            var packageImageRepo = _unitOfWork.GetRepo<PackageImage>();
            var any = await repo.AnyAsync(new QueryBuilder<AdvertisementPackage>()
                .WithPredicate(x => x.Id == dto.Id)
                .WithInclude(x => x.PackageImages)
                .Build());
            if (any)
            {
                var adPackage = await repo.GetSingleAsync(new QueryBuilder<AdvertisementPackage>()
                    .WithPredicate(x => x.Id == dto.Id)
                    .WithTracking(false)
                    .Build());
                var updatePackageDto = new AdvertisementPackageUpdateDTO
                {
                    Name = dto.Name,
                    Price = dto.Price,
                    Description = dto.Description,
                    LimitContent = dto.LimitContent,
                    LimitImage = dto.LimitImage,
                    LimitAd = dto.LimitAd
                };
                var updatePackage = _mapper.Map(updatePackageDto, adPackage);
                await repo.UpdateAsync(updatePackage);
            }
            else
            {
                var createdPackageDto = new AdvertisementPackageCreateDTO()
                {
                    Name = dto.Name,
                    Price = dto.Price,
                    Description = dto.Description,
                    LimitContent = dto.LimitContent,
                    LimitImage = dto.LimitImage,
                    LimitAd = dto.LimitAd
                };
                var createdPackage = _mapper.Map<AdvertisementPackage>(createdPackageDto);
                createdPackage.CreatedBy = userName;
                createdPackage.IsActive = true;
                createdPackage.CreatedDate = DateTime.Now;
                await repo.CreateAsync(createdPackage);
                await _unitOfWork.SaveChangesAsync();

                if (dto.ImageIds != null || dto.ImageIds.Any())
                {
                    var createdPackageImageDTOs = new List<PackageImageCreateDTO>();
                    foreach (var image in dto.ImageIds)
                    {
                        var existedImage = await _unitOfWork.GetRepo<Image>().GetSingleAsync(new QueryBuilder<Image>()
                                                                                             .WithPredicate(x => x.Id == image)
                                                                                             .WithTracking(false)
                                                                                             .Build());
                        if(existedImage == null)  return new BaseResponse { IsSuccess = false, Message = $"Ảnh {image} không tồn tại."};
                        var createdPackageImageDto = new PackageImageCreateDTO
                        {
                            AdvertisementPackageId = createdPackage.Id,
                            ImageId = image
                        };
                        createdPackageImageDTOs.Add(createdPackageImageDto);
                    }
                    var createdPackageImages = _mapper.Map<List<PackageImage>>(createdPackageImageDTOs);
                    await packageImageRepo.CreateAllAsync(createdPackageImages);
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

	public async Task<BaseResponse> DeleteImagesFromoPackage(PackageImageDeleteDTO dto)
	{
		try
		{
			await _unitOfWork.BeginTransactionAsync();
			var repo = _unitOfWork.GetRepo<PackageImage>();
			var packageRepo = _unitOfWork.GetRepo<AdvertisementPackage>();
			var deletedPackageImages = new List<PackageImage>();
			var any = await packageRepo.AnyAsync(new QueryBuilder<AdvertisementPackage>()
				.WithPredicate(x => x.Id == dto.AdvertisementPackageId)
				.WithTracking(false)
				.Build());
			if (any)
			{
				foreach (var packageImageId in dto.PackageImageIds)
				{
					var deletePackageImage = await repo.GetSingleAsync(new QueryBuilder<PackageImage>()
						.WithPredicate(x => x.Id == packageImageId && x.AdvertisementPackageId == dto.AdvertisementPackageId)
						.WithTracking(false)
						.Build());
					if (deletePackageImage == null)
						return new BaseResponse
						{ IsSuccess = false, Message = $"Ảnh {packageImageId} không tồn tại trong gói" };
					deletedPackageImages.Add(deletePackageImage);
				}

				await repo.DeleteAllAsync(deletedPackageImages);
				var saver = await _unitOfWork.SaveAsync();
				await _unitOfWork.CommitTransactionAsync();
				if (!saver) return new BaseResponse { IsSuccess = false, Message = "Lưu dữ liệu thất bại" };
				return new BaseResponse { IsSuccess = true, Message = "Lưu dữ liệu thành công." };
			}
			return new BaseResponse { IsSuccess = false, Message = "Gói không tồn tại." };
		}
		catch (Exception)
		{
			await _unitOfWork.RollBackAsync();
			throw;
		}
	}
}