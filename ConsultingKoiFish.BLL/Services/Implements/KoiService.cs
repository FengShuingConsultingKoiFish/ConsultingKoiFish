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

    public async Task<BaseResponse> AddSuitableKoiZodiac(ZodiacKoiBreedDTO zodiacKoiBreedDto)
    {
        try
        {
            var repo = _unitOfWork.GetRepo<KoiBreedZodiac>();
            var zodiacKoiBreed = new KoiBreedZodiac
            {
                KoiBreedId = zodiacKoiBreedDto.KoiBreedId,
                ZodiacId = zodiacKoiBreedDto.ZodiacId,
               
            };
            
            await repo.CreateAsync(zodiacKoiBreed);
            await _unitOfWork.SaveChangesAsync();
            
            return new BaseResponse { IsSuccess = true, Message = "Thêm tương hợp cung hoàng đạo-Koi thành công" };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new BaseResponse { IsSuccess = false, Message = "Có lỗi xảy ra khi thêm tương hợp cung hoàng đạo-Koi" };
        }
    }

    public async Task<BaseResponse> UpdateKoiZodiac(ZodiacKoiBreedDTO zodiacKoiBreedDto, int koiZodiacId)
    {
        try
        {
            var repo = _unitOfWork.GetRepo<KoiBreedZodiac>();
            
            await _unitOfWork.BeginTransactionAsync();
            
            var zodiacExists = await repo.AnyAsync(new QueryBuilder<KoiBreedZodiac>()
                .WithPredicate(x => x.Id.Equals(koiZodiacId))
                .Build());

            if (!zodiacExists)
            {
                return new BaseResponse { IsSuccess = false, Message = "Không tìm thấy tương hợp cung hoàng đạo-Koi." };
            }
            
            var zodiacKoiBreedDetail = await repo.GetSingleAsync(new QueryBuilder<KoiBreedZodiac>()
                .WithPredicate(x => x.Id.Equals(koiZodiacId))
                .Build());
            
            zodiacKoiBreedDetail.KoiBreedId = zodiacKoiBreedDto.KoiBreedId;
            zodiacKoiBreedDetail.ZodiacId = zodiacKoiBreedDto.ZodiacId;
            
            await repo.UpdateAsync(zodiacKoiBreedDetail);
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

    public async Task<ResponseApiDTO> GetAllKoiZodiac()
    {
        try
        {
            var queryBuilder = new QueryBuilder<KoiBreedZodiac>()
                .WithOrderBy(q => q.OrderBy(k => k.KoiBreedId))
                .WithTracking(false);
                
            var repo = _unitOfWork.GetRepo<KoiBreedZodiac>();
            var zodiacKoiBreeds = await repo.GetAllAsync(queryBuilder.Build());

            var zodiacKoiBreedDTOs = _mapper.Map<List<KoiBreedZodiac>>(zodiacKoiBreeds);
            return new ResponseApiDTO { IsSuccess = true, Result = zodiacKoiBreedDTOs };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new ResponseApiDTO { IsSuccess = false, Message = "Không thể lấy danh sách tương hợp cung hoàng đạo-Koi." };
        }
    }

    public async Task<BaseResponse> DeleteKoiZodiac(int koiZodiacId)
    {
        try
        {
            var repo = _unitOfWork.GetRepo<KoiBreedZodiac>();
            var zodiacKoiBreed = await repo.GetSingleAsync(new QueryBuilder<KoiBreedZodiac>()
                .WithPredicate(x => x.Id == koiZodiacId)
                .Build());

            if (zodiacKoiBreed == null)
            {
                return new BaseResponse { IsSuccess = false, Message = "Không tìm thấy tương hợp cung hoàng đạo-Koi." };
            }

            await repo.DeleteAsync(zodiacKoiBreed);
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
            return new BaseResponse { IsSuccess = false, Message = "Có lỗi xảy ra khi xóa tương hợp cung hoàng đạo-Koi." };
        }
    }

    public async Task<ResponseApiDTO> GetSuitableKoiForUser(string userId)
{
    try
    {
        // Step 1: Retrieve user's zodiac sign from the UserZodiac repository
        var userZodiacRepo = _unitOfWork.GetRepo<UserZodiac>();
        var userZodiac = await userZodiacRepo.GetSingleAsync(new QueryBuilder<UserZodiac>()
            .WithPredicate(x => x.UserId.Equals(userId))
            .Build());

        if (userZodiac == null)
        {
            return new ResponseApiDTO { IsSuccess = false, Message = "Zodiac for the user not found." };
        }

        // Step 2: Retrieve matching Koi breeds for the user's zodiac, including KoiBreed and Zodiac details
        var koiZodiacRepo = _unitOfWork.GetRepo<KoiBreedZodiac>();
        var queryBuilder = new QueryBuilder<KoiBreedZodiac>()
            .WithPredicate(x => x.ZodiacId == userZodiac.ZodiacId)  // Match the ZodiacId from UserZodiac
            .WithInclude(x => x.KoiBreed) // Include KoiBreed details
            .WithInclude(x => x.Zodiac) // Include Zodiac details
            .Build();

        var suitableKoiBreeds = await koiZodiacRepo.GetAllAsync(queryBuilder);
        if (suitableKoiBreeds == null || !suitableKoiBreeds.Any())
        {
            return new ResponseApiDTO { IsSuccess = false, Message = "No suitable Koi breeds found for the user's zodiac." };
        }

        // Step 3: Map the result to a DTO including KoiBreed and Zodiac names
        var suitableKoiBreedDTOs = suitableKoiBreeds.Select(koiBreedZodiac => new 
        {
            KoiBreedId = koiBreedZodiac.KoiBreedId,
            KoiBreedName = koiBreedZodiac.KoiBreed?.Name,
            ZodiacId = koiBreedZodiac.ZodiacId,
            ZodiacName = koiBreedZodiac.Zodiac?.ZodiacName // Ensure "Name" is the correct field name
        }).ToList();

        return new ResponseApiDTO { IsSuccess = true, Result = suitableKoiBreedDTOs, Message = "Suitable Koi breeds retrieved successfully." };
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
        return new ResponseApiDTO { IsSuccess = false, Message = "An error occurred while retrieving suitable Koi breeds." };
    }
}


}
