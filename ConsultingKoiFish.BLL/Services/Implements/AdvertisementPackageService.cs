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
}