using AutoMapper;
using ConsultingKoiFish.BLL.DTOs;
using ConsultingKoiFish.BLL.DTOs.Response;
using ConsultingKoiFish.BLL.Services.Interfaces;
using ConsultingKoiFish.DAL.Entities;
using ConsultingKoiFish.DAL.Queries;
using ConsultingKoiFish.DAL.UnitOfWork;
using Microsoft.AspNetCore.Builder;

namespace ConsultingKoiFish.BLL.Services.Implements;

public class UserPondService : IUserPondService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UserPondService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        this._unitOfWork = unitOfWork;
        this._mapper = mapper;
    }
    public async Task<BaseResponse> AddUserPond(UserPondDTOs userPondDtOs, string userId)
    {
        try
        {
            var repo = _unitOfWork.GetRepo<UserPond>();
            var zodiacDTO = new UserPond()
            {
                UserId = userId,
                PondName = userPondDtOs.PondName,
                Quantity = userPondDtOs.Quantity,
                Description = userPondDtOs.Description,
                Image = userPondDtOs.Image
            };
            
            await repo.CreateAsync(zodiacDTO);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponse { IsSuccess = true, Message = "Thêm hồ thành công thành công" };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<BaseResponse> UpdateUserPond(UserPondDTOs userPondDtOs, int userPondId)
    {
        try
        {
            var repo = _unitOfWork.GetRepo<UserPond>();
            
            await _unitOfWork.BeginTransactionAsync();
            
            var zodiacExists = await repo.AnyAsync(new QueryBuilder<UserPond>()
                .WithPredicate(x => x.Id.Equals(userPondId))
                .Build());

            if (!zodiacExists)
            {
                return new BaseResponse { IsSuccess = false, Message = "Zodiac không tồn tại." };
            }
            
            var userPond = await repo.GetSingleAsync(new QueryBuilder<UserPond>()
                .WithPredicate(x => x.Id.Equals(userPondId))
                .Build());
            
            userPond.PondName = userPondDtOs.PondName;
            userPond.Quantity = userPondDtOs.Quantity;
            userPond.Description = userPondDtOs.Description;
            userPond.Image = userPondDtOs.Image;
            userPond.Score = 0;
            
            await repo.UpdateAsync(userPond);
            
            var isSaved = await _unitOfWork.SaveAsync();
            
            await _unitOfWork.CommitTransactionAsync();

            if (!isSaved)
            {
                return new BaseResponse { IsSuccess = false, Message = "Lưu dữ liệu thất bại" };
            }

            return new BaseResponse { IsSuccess = true, Message = "Lưu dữ liệu thành công" };
        }
        catch (Exception e)
        {
            //await _unitOfWork.RollbackTransactionAsync();
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<ResponseApiDTO> GetAllUserPond(string userId)
    {
        try
        {
            // Lấy repository của UserPond
            var repo = _unitOfWork.GetRepo<UserPond>();

            // Truy vấn danh sách hồ của user theo UserId
            var userPonds = await repo.GetAllAsync(new QueryBuilder<UserPond>()
                .WithPredicate(x => x.UserId == userId)
                .Build());

            // Kiểm tra nếu không tìm thấy hồ nào
            if (userPonds == null || !userPonds.Any())
            {
                return new ResponseApiDTO
                {
                    IsSuccess = false,
                    Message = "Không tìm thấy hồ nào cho người dùng này.",
                    
                };
            }

            // Áp dụng AutoMapper để chuyển đổi từ thực thể sang DTO
            var userPondDtos = _mapper.Map<IEnumerable<UserPond>>(userPonds);

            // Trả về danh sách DTO
            return new ResponseApiDTO
            {
                IsSuccess = true,
                Message = "Lấy danh sách hồ thành công.",
                Result = userPondDtos
            };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new ResponseApiDTO
            {
                IsSuccess = false,
                Message = "Có lỗi xảy ra khi lấy danh sách hồ.",
                Result = null
            };
        }
    }


    public async Task<BaseResponse> DeleteUserPond(int userPondId)
{
    try
    {
        // Get repository for UserPond, KoiDetail, and PondDetail
        var userPondRepo = _unitOfWork.GetRepo<UserPond>();
        var koiDetailRepo = _unitOfWork.GetRepo<KoiDetail>();
        var pondDetailRepo = _unitOfWork.GetRepo<PondDetail>();

        // Check if UserPond exists
        var userPond = await userPondRepo.GetSingleAsync(new QueryBuilder<UserPond>()
            .WithPredicate(x => x.Id == userPondId)
            .Build());

        if (userPond == null)
        {
            return new BaseResponse
            {
                IsSuccess = false,
                Message = "Hồ không tồn tại."
            };
        }

        // Retrieve and delete all associated KoiDetails
        var koiDetails = await koiDetailRepo.GetAllAsync(new QueryBuilder<KoiDetail>()
            .WithPredicate(x => x.UserPondId == userPondId)
            .Build());

        foreach (var koiDetail in koiDetails)
        {
            await koiDetailRepo.DeleteAsync(koiDetail);
        }

        // Retrieve and delete all associated PondDetails
        var pondDetails = await pondDetailRepo.GetAllAsync(new QueryBuilder<PondDetail>()
            .WithPredicate(x => x.UserPondId == userPondId)
            .Build());

        foreach (var pondDetail in pondDetails)
        {
            await pondDetailRepo.DeleteAsync(pondDetail);
        }

        // Finally, delete the UserPond itself
        await userPondRepo.DeleteAsync(userPond);

        // Save all changes
        var isDeleted = await _unitOfWork.SaveAsync();

        if (!isDeleted)
        {
            return new BaseResponse
            {
                IsSuccess = false,
                Message = "Xóa hồ thất bại."
            };
        }

        return new BaseResponse
        {
            IsSuccess = true,
            Message = "Xóa hồ và các chi tiết liên quan thành công."
        };
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
        return new BaseResponse
        {
            IsSuccess = false,
            Message = "Có lỗi xảy ra khi xóa hồ và các chi tiết liên quan."
        };
    }
}

    

    public async Task<ResponseApiDTO> AddKoiAndPondDetails(KoiAndPondDetailRequestDTOs.KoiAndPondDetailRequestDTO requestDto)
{
    try
    {
        var userPondRepo = _unitOfWork.GetRepo<UserPond>();
        var koiDetailRepo = _unitOfWork.GetRepo<KoiDetail>();
        var pondDetailRepo = _unitOfWork.GetRepo<PondDetail>();
        var koiBreedZodiacRepo = _unitOfWork.GetRepo<KoiBreedZodiac>();
        var pondZodiacRepo = _unitOfWork.GetRepo<PondZodiac>();
        var userZodiacRepo = _unitOfWork.GetRepo<UserZodiac>();

        // Validate if UserPond exists for the provided PondId and get the userId
        var userPond = await userPondRepo.GetSingleAsync(new QueryBuilder<UserPond>()
            .WithPredicate(x => x.Id == requestDto.PondId)
            .Build());

        if (userPond == null)
        {
            return new ResponseApiDTO
            {
                IsSuccess = false,
                Message = "UserPond không tồn tại."
            };
        }

        // Retrieve ZodiacId associated with the userId from UserPond
        var userZodiac = await userZodiacRepo.GetSingleAsync(new QueryBuilder<UserZodiac>()
            .WithPredicate(x => x.UserId == userPond.UserId)
            .Build());

        if (userZodiac == null)
        {
            return new ResponseApiDTO
            {
                IsSuccess = false,
                Message = "Zodiac không tồn tại cho người dùng này."
            };
        }

        // Get the total number of Koi already added to this pond
        var existingKoiDetails = await koiDetailRepo.GetAllAsync(new QueryBuilder<KoiDetail>()
            .WithPredicate(x => x.UserPondId == requestDto.PondId)
            .Build());

        var existingKoiCount = existingKoiDetails.Count();


        // Ensure the total koi in the pond does not exceed the maximum allowed
        if (requestDto.KoiDetails != null && requestDto.KoiDetails.Any())
        {
            int totalKoiToAdd = requestDto.KoiDetails.Count;
            if (existingKoiCount + totalKoiToAdd > userPond.Quantity)
            {
                return new ResponseApiDTO
                {
                    IsSuccess = false,
                    Message = $"Không thể thêm quá số lượng cá Koi cho phép trong hồ, số lượng tối đa: {userPond.Quantity}"
                };
            }

            // Map KoiDetails and add to the repository
            foreach (var koiDetailDto in requestDto.KoiDetails)
            {
                var koiDetail = new KoiDetail
                {
                    UserPondId = requestDto.PondId, // Set UserPondId from request
                    KoiBreedId = koiDetailDto.KoiBreedId
                };
                await koiDetailRepo.CreateAsync(koiDetail);
            }
        }

        // Check and add pond details if provided
        if (requestDto.PondDetails != null && requestDto.PondDetails.Any())
        {
            // Map PondDetails and add to the repository
            foreach (var pondDetailDto in requestDto.PondDetails)
            {
                var pondDetail = new PondDetail
                {
                    UserPondId = requestDto.PondId, // Set UserPondId from request
                    PondId = pondDetailDto.PondId
                };
                await pondDetailRepo.CreateAsync(pondDetail);
            }
        }

        // Save all changes
        await _unitOfWork.SaveChangesAsync();

        // Calculate the score based on matching ponds and koi using ZodiacId
        var matchingPonds = await pondZodiacRepo.GetAllAsync(new QueryBuilder<PondZodiac>()
            .WithPredicate(x => x.ZodiacId == userZodiac.ZodiacId)
            .Build());
        var matchingPondsCount = matchingPonds.Count();

        var matchingKoi = await koiBreedZodiacRepo.GetAllAsync(new QueryBuilder<KoiBreedZodiac>()
            .WithPredicate(x => x.ZodiacId == userZodiac.ZodiacId)
            .Build());
        var matchingKoiCount = matchingKoi.Count();

        // Assuming a scoring logic where each match adds 10 points
        userPond.Score = (matchingPondsCount + matchingKoiCount) * 10;

        // Update the UserPond score in the database
        await userPondRepo.UpdateAsync(userPond);
        var isSaved = await _unitOfWork.SaveAsync();

        if (!isSaved)
        {
            return new ResponseApiDTO
            {
                IsSuccess = false,
                Message = "Không thể cập nhật điểm số sau khi thêm chi tiết Koi và hồ."
            };
        }

        return new ResponseApiDTO
        {
            IsSuccess = true,
            Message = $"Thêm chi tiết Koi và hồ thành công, điểm số cho hồ cá Koi của bạn là {userPond.Score}"
        };
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
        return new ResponseApiDTO
        {
            IsSuccess = false,
            Message = "Có lỗi xảy ra khi thêm chi tiết Koi và hồ."
        };
    }
}

    public async Task<BaseResponse> UpdateKoiAndPondDetails(int userPondId, KoiAndPondDetailRequestDTOs.KoiAndPondDetailRequestDTO requestDto)
{
    try
    {
        var userPondRepo = _unitOfWork.GetRepo<UserPond>();
        var koiDetailRepo = _unitOfWork.GetRepo<KoiDetail>();
        var pondDetailRepo = _unitOfWork.GetRepo<PondDetail>();

        // Check if UserPond exists
        var userPondExists = await userPondRepo.AnyAsync(new QueryBuilder<UserPond>()
            .WithPredicate(x => x.Id == userPondId)
            .Build());

        if (!userPondExists)
        {
            return new BaseResponse
            {
                IsSuccess = false,
                Message = "UserPond không tồn tại."
            };
        }

        // Delete existing KoiDetails and PondDetails associated with UserPondId
        var existingKoiDetails = await koiDetailRepo.GetAllAsync(new QueryBuilder<KoiDetail>()
            .WithPredicate(x => x.UserPondId == userPondId)
            .Build());

        var existingPondDetails = await pondDetailRepo.GetAllAsync(new QueryBuilder<PondDetail>()
            .WithPredicate(x => x.UserPondId == userPondId)
            .Build());

        foreach (var koiDetail in existingKoiDetails)
            await koiDetailRepo.DeleteAsync(koiDetail);

        foreach (var pondDetail in existingPondDetails)
            await pondDetailRepo.DeleteAsync(pondDetail);

        // Add updated KoiDetails
        foreach (var koiDetailDto in requestDto.KoiDetails)
        {
            var koiDetail = new KoiDetail
            {
                UserPondId = userPondId,
                KoiBreedId = koiDetailDto.KoiBreedId
            };
            await koiDetailRepo.CreateAsync(koiDetail);
        }

        // Add updated PondDetails
        foreach (var pondDetailDto in requestDto.PondDetails)
        {
            var pondDetail = new PondDetail
            {
                UserPondId = userPondId,
                PondId = pondDetailDto.PondId
            };
            await pondDetailRepo.CreateAsync(pondDetail);
        }

        // Save changes to database
        await _unitOfWork.SaveChangesAsync();

        return new BaseResponse
        {
            IsSuccess = true,
            Message = "Cập nhật chi tiết Koi và hồ thành công."
        };
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
        return new BaseResponse
        {
            IsSuccess = false,
            Message = "Có lỗi xảy ra khi cập nhật chi tiết Koi và hồ."
        };
    }
}
    public async Task<ResponseApiDTO> ViewKoiAndPondDetails(int userPondId)
{
    try
    {
        // Repositories for fetching Koi and Pond details
        var koiDetailRepo = _unitOfWork.GetRepo<KoiDetail>();
        var pondDetailRepo = _unitOfWork.GetRepo<PondDetail>();
        var pondCategoryRepo = _unitOfWork.GetRepo<PondCategory>();
        var koiCategoryRepo = _unitOfWork.GetRepo<KoiCategory>();

        // Fetch Koi details
        var koiDetails = await koiDetailRepo.GetAllAsync(new QueryBuilder<KoiDetail>()
            .WithPredicate(x => x.UserPondId == userPondId)
            .WithInclude(x => x.KoiBreed)
            .Build());

        // Fetch Pond details
        var pondDetails = await pondDetailRepo.GetAllAsync(new QueryBuilder<PondDetail>()
            .WithPredicate(x => x.UserPondId == userPondId)
            .WithInclude(x => x.Pond)
            .Build());

        // Step 3: Check if there are any Koi or Pond details
        if ((koiDetails == null || !koiDetails.Any()) && (pondDetails == null || !pondDetails.Any()))
        {
            return new ResponseApiDTO
            {
                IsSuccess = false,
                Message = "Không tìm thấy chi tiết Koi hoặc hồ cho UserPond này.",
                Result = null
            };
        }

        // Step 4: Fetch pond categories in bulk to minimize repeated queries
        var pondCategoryIds = pondDetails.Select(pd => pd.Pond.PondCategoryId).Distinct().ToList();
        var pondCategories = await pondCategoryRepo.GetAllAsync(new QueryBuilder<PondCategory>()
            .WithPredicate(pc => pondCategoryIds.Contains(pc.Id))
            .Build());

        var koiCategoryIds = koiDetails.Select(kd => kd.KoiBreed.KoiCategoryId).Distinct().ToList();
        var koiCategories = await koiCategoryRepo.GetAllAsync(new QueryBuilder<KoiCategory>()
            .WithPredicate(kc => koiCategoryIds.Contains(kc.Id))
            .Build());

        // Step 5: Map Koi details to DTO, including fetching KoiCategory name
        var koiDetailDtos = koiDetails.Select(koiDetail =>
        {
            var koiCategoryName = koiCategories.FirstOrDefault(kc => kc.Id == koiDetail.KoiBreed?.KoiCategoryId)?.Name ?? "Unknown Category";
            return new 
            {
                KoiDetailId = koiDetail.Id,
                KoiBreedName = koiDetail.KoiBreed?.Name ?? "Unknown Breed",
                KoiCategory = koiCategoryName,
                Image = koiDetail.KoiBreed?.Image,
            };
        }).ToList();

        // Step 6: Map Pond details to DTO, including fetching PondCategory name
        var pondDetailDtos = pondDetails.Select(pondDetail =>
        {
            var pondCategoryName = pondCategories.FirstOrDefault(pc => pc.Id == pondDetail.Pond?.PondCategoryId)?.Name ?? "Unknown Category";
            return new 
            {
                PondDetailId = pondDetail.Id,
                PondName = pondDetail.Pond?.Name ?? "Unknown Pond",
                PondCategory = pondCategoryName,
                Image = pondDetail.Pond?.Image,
            };
        }).ToList();

        // Step 7: Return the result
        return new ResponseApiDTO
        {
            IsSuccess = true,
            Message = "Lấy chi tiết Koi và hồ thành công.",
            Result = new
            {
                KoiDetails = koiDetailDtos,
                PondDetails = pondDetailDtos
            }
        };
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
        return new ResponseApiDTO
        {
            IsSuccess = false,
            Message = "Đã xảy ra lỗi khi lấy chi tiết Koi và hồ.",
            Result = null
        };
    }
}
    public async Task<BaseResponse> DeleteKoiBreedFromUserPond(int userPondId, int koiBreedId)
    {
        try
        {
            var koiDetailRepo = _unitOfWork.GetRepo<KoiDetail>();

            // Find the koi detail that matches the given userPondId and koiBreedId
            var koiDetail = await koiDetailRepo.GetSingleAsync(new QueryBuilder<KoiDetail>()
                .WithPredicate(x => x.UserPondId == userPondId && x.KoiBreedId == koiBreedId)
                .Build());

            if (koiDetail == null)
            {
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = "Không tìm thấy chi tiết cá Koi để xóa."
                };
            }

            // Delete the koi detail
            await koiDetailRepo.DeleteAsync(koiDetail);

            // Save changes
            var isDeleted = await _unitOfWork.SaveAsync();

            if (!isDeleted)
            {
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = "Xóa cá Koi thất bại."
                };
            }

            return new BaseResponse
            {
                IsSuccess = true,
                Message = "Xóa cá Koi thành công."
            };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new BaseResponse
            {
                IsSuccess = false,
                Message = "Có lỗi xảy ra khi xóa cá Koi."
            };
        }
    }
    public async Task<BaseResponse> DeletePondFromUserPond(int userPondId, int pondId)
    {
        try
        {
            var pondDetailRepo = _unitOfWork.GetRepo<PondDetail>();

            // Find the pond detail that matches the given userPondId and pondId
            var pondDetail = await pondDetailRepo.GetSingleAsync(new QueryBuilder<PondDetail>()
                .WithPredicate(x => x.UserPondId == userPondId && x.PondId == pondId)
                .Build());

            if (pondDetail == null)
            {
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = "Không tìm thấy chi tiết hồ để xóa."
                };
            }

            // Delete the pond detail
            await pondDetailRepo.DeleteAsync(pondDetail);

            // Save changes
            var isDeleted = await _unitOfWork.SaveAsync();

            if (!isDeleted)
            {
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = "Xóa hồ thất bại."
                };
            }

            return new BaseResponse
            {
                IsSuccess = true,
                Message = "Xóa hồ thành công."
            };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new BaseResponse
            {
                IsSuccess = false,
                Message = "Có lỗi xảy ra khi xóa hồ."
            };
        }
    }




    
    





}