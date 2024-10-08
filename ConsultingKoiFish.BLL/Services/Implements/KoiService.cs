using System.Net;
using AutoMapper;
using ConsultingKoiFish.BLL.DTOs.KoiDTOs;
using ConsultingKoiFish.BLL.DTOs.Response;
using ConsultingKoiFish.BLL.Services.Interfaces;
using ConsultingKoiFish.DAL.Entities;
using ConsultingKoiFish.DAL.Queries;
using ConsultingKoiFish.DAL.UnitOfWork;

namespace ConsultingKoiFish.BLL.Services.Implements;

public class KoiService : IKoiService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public KoiService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        this._unitOfWork = unitOfWork;
        this._mapper = mapper;
    }

    public async Task<BaseResponse> AddKoiCategory(KoiCategoryDTO koiCategoryDto)
    {
        try
        {
            var repo = _unitOfWork.GetRepo<KoiCategory>();
            var koiCategory = new KoiCategory()
            {
                Name = koiCategoryDto.Name,
                Description = koiCategoryDto.Description
                
            };
            await repo.CreateAsync(koiCategory);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponse { IsSuccess = true, Message = "Thêm danh mục Koi thành công" };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new BaseResponse { IsSuccess = false, Message = "Thêm danh mục Koi thất bại." };
        }
    }

    public async Task<BaseResponse> UpdateKoiCategory(KoiCategoryDTO koiCategoryDto, int koiCategoryId)
    {
        try
        {
            var repo = _unitOfWork.GetRepo<KoiCategory>();
            
            await _unitOfWork.BeginTransactionAsync();
            
            var categoryExists = await repo.AnyAsync(new QueryBuilder<KoiCategory>()
                .WithPredicate(x => x.Id.Equals(koiCategoryId))
                .Build());

            if (!categoryExists)
            {
                return new BaseResponse { IsSuccess = false, Message = "Danh mục Koi không tồn tại." };
            }
            
            var koiCategoryDetail = await repo.GetSingleAsync(new QueryBuilder<KoiCategory>()
                .WithPredicate(x => x.Id.Equals(koiCategoryId))
                .Build());
            
            koiCategoryDetail.Name = koiCategoryDto.Name;
            koiCategoryDetail.Description = koiCategoryDto.Description;
            
            await repo.UpdateAsync(koiCategoryDetail);
            
            var isSaved = await _unitOfWork.SaveAsync();
            
            await _unitOfWork.CommitTransactionAsync();

            if (!isSaved)
            {
                return new BaseResponse { IsSuccess = false, Message = "Cập nhật thất bại." };
            }

            return new BaseResponse { IsSuccess = true, Message = "Cập nhật thành công." };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new BaseResponse { IsSuccess = false, Message = "Có lỗi xảy ra trong quá trình cập nhật." };
        }
    }

    public async Task<ResponseApiDTO> GetAllKoiCategory()
    {
        try
        {
            var queryBuilder = new QueryBuilder<KoiCategory>()
                .WithOrderBy(q => q.OrderBy(k => k.Name))  
                .WithTracking(false);  
            
            var repo = _unitOfWork.GetRepo<KoiCategory>();
            var koiCategories = await repo.GetAllAsync(queryBuilder.Build());
            
            var koiCategoryDTOs = _mapper.Map<List<KoiCategory>>(koiCategories);
            return new ResponseApiDTO { IsSuccess = true, Result = koiCategoryDTOs };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new ResponseApiDTO { IsSuccess = false, Message = "Không thể lấy danh sách danh mục Koi." };
        }
    }

    public async Task<BaseResponse> DeleteKoiCategory(int koiCategoryId)
    {
        try
        {
            var repo = _unitOfWork.GetRepo<KoiCategory>();
            var koiCategory = await repo.GetSingleAsync(new QueryBuilder<KoiCategory>()
                .WithPredicate(x => x.Id == koiCategoryId)
                .Build());

            if (koiCategory == null)
            {
                return new BaseResponse { IsSuccess = false, Message = "Không tìm thấy danh mục Koi." };
            }

            await repo.DeleteAsync(koiCategory);
            var isSaved = await _unitOfWork.SaveAsync();

            if (!isSaved) 
                return new BaseResponse { IsSuccess = false, Message = "Xóa thất bại." };

            return new BaseResponse { IsSuccess = true, Message = "Xóa thành công." };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new BaseResponse { IsSuccess = false, Message = "Có lỗi xảy ra khi xóa danh mục Koi." };
        }
    }

    public async Task<BaseResponse> AddKoiBreed(KoiBreedDTO koiBreedDto)
    {
        try
        {
            var repo = _unitOfWork.GetRepo<KoiBreed>();
            var koiBreed = new KoiBreed()
            {
                KoiCategoryId = koiBreedDto.KoiCategoryId,
                Name = koiBreedDto.Name,
                Colors = koiBreedDto.Colors,
                Pattern = koiBreedDto.Pattern,
                Description = koiBreedDto.Description,
                Image = koiBreedDto.Image
            };
            await repo.CreateAsync(koiBreed);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponse { IsSuccess = true, Message = "Thêm giống Koi thành công" };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new BaseResponse { IsSuccess = false, Message = "Có lỗi xảy ra khi thêm giống Koi" };
        }
    }

    public async Task<BaseResponse> UpdateKoiBreed(KoiBreedDTO koiBreedDto, int koiBreedId)
    {
        try
        {
            var repo = _unitOfWork.GetRepo<KoiBreed>();
                
            await _unitOfWork.BeginTransactionAsync();
                
            var breedExists = await repo.AnyAsync(new QueryBuilder<KoiBreed>()
                .WithPredicate(x => x.Id.Equals(koiBreedId))
                .Build());

            if (!breedExists)
            {
                return new BaseResponse { IsSuccess = false, Message = "Giống Koi không tồn tại." };
            }
                
            var koiBreedDetail = await repo.GetSingleAsync(new QueryBuilder<KoiBreed>()
                .WithPredicate(x => x.Id.Equals(koiBreedId))
                .Build());

            koiBreedDetail.KoiCategoryId = koiBreedDto.KoiCategoryId;
            koiBreedDetail.Name = koiBreedDto.Name;
            koiBreedDetail.Colors = koiBreedDto.Colors;
            koiBreedDetail.Pattern = koiBreedDto.Pattern;
            koiBreedDetail.Description = koiBreedDto.Description;
            koiBreedDetail.Image = koiBreedDto.Image;

            await repo.UpdateAsync(koiBreedDetail);
            var isSaved = await _unitOfWork.SaveAsync();

            await _unitOfWork.CommitTransactionAsync();

            if (!isSaved)
            {
                return new BaseResponse { IsSuccess = false, Message = "Cập nhật thông tin thất bại." };
            }

            return new BaseResponse { IsSuccess = true, Message = "Cập nhật thông tin thành công." };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new BaseResponse { IsSuccess = false, Message = "Có lỗi xảy ra trong quá trình cập nhật." };
        }
    }

    public async Task<ResponseApiDTO> GetAllKoiBreed()
    {
        try
        {
            var queryBuilder = new QueryBuilder<KoiBreed>()
                .WithOrderBy(q => q.OrderBy(k => k.Name))
                .WithTracking(false);
                
            var repo = _unitOfWork.GetRepo<KoiBreed>();
            var koiBreeds = await repo.GetAllAsync(queryBuilder.Build());

            var koiBreedDTOs = _mapper.Map<List<KoiBreed>>(koiBreeds);
            return new ResponseApiDTO { IsSuccess = true, Result = koiBreedDTOs };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new ResponseApiDTO { IsSuccess = false, Message = "Không thể lấy danh sách giống Koi." };
        }
    }

    public async Task<BaseResponse> DeleteKoiBreed(int koiBreedId)
    {
        try
        {
            var repo = _unitOfWork.GetRepo<KoiBreed>();
            var koiBreed = await repo.GetSingleAsync(new QueryBuilder<KoiBreed>()
                .WithPredicate(x => x.Id == koiBreedId)
                .Build());

            if (koiBreed == null)
            {
                return new BaseResponse { IsSuccess = false, Message = "Không tìm thấy giống Koi." };
            }

            await repo.DeleteAsync(koiBreed);
            var isSaved = await _unitOfWork.SaveAsync();

            if (!isSaved)
            {
                return new BaseResponse { IsSuccess = false, Message = "Xóa thất bại." };
            }

            return new BaseResponse { IsSuccess = true, Message = "Xóa thành công." };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new BaseResponse { IsSuccess = false, Message = "Có lỗi xảy ra khi xóa giống Koi." };
        }
    }
}
