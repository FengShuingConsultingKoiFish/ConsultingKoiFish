using AutoMapper;
using ConsultingKoiFish.BLL.DTOs;
using ConsultingKoiFish.BLL.DTOs.Response;
using ConsultingKoiFish.BLL.Services.Interfaces;
using ConsultingKoiFish.DAL.Entities;
using ConsultingKoiFish.DAL.Queries;
using ConsultingKoiFish.DAL.UnitOfWork;

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
    public async Task<BaseResponse> AddUserPond(UserPondDTOs userPondDtOs)
    {
        try
        {
            var repo = _unitOfWork.GetRepo<UserPond>();
            var zodiacDTO = new UserPond()
            {
                UserId = userPondDtOs.UserId,
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
            var userPondDtos = _mapper.Map<IEnumerable<UserPondDTOs>>(userPonds);

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

    public async Task<BaseResponse> AddKoiAndPondDetails(KoiAndPondDetailRequestDTOs.KoiAndPondDetailRequestDTO requestDto)
    {
        try
        {
            var userPondRepo = _unitOfWork.GetRepo<UserPond>();
            var koiDetailRepo = _unitOfWork.GetRepo<KoiDetail>();
            var pondDetailRepo = _unitOfWork.GetRepo<PondDetail>();

            // Validate if UserPond exists for the provided PondId
            var userPondExists = await userPondRepo.AnyAsync(new QueryBuilder<UserPond>()
                .WithPredicate(x => x.Id == requestDto.PondId)
                .Build());

            if (!userPondExists)
            {
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = "UserPond không tồn tại."
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

            // Save changes to database
            await _unitOfWork.SaveChangesAsync();

            return new BaseResponse
            {
                IsSuccess = true,
                Message = "Thêm chi tiết Koi và hồ thành công."
            };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new BaseResponse
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
            var koiDetailRepo = _unitOfWork.GetRepo<KoiDetail>();
            var pondDetailRepo = _unitOfWork.GetRepo<PondDetail>();

            // Get KoiDetails and PondDetails for the UserPond
            var koiDetails = await koiDetailRepo.GetAllAsync(new QueryBuilder<KoiDetail>()
                .WithPredicate(x => x.UserPondId == userPondId)
                .Build());

            var pondDetails = await pondDetailRepo.GetAllAsync(new QueryBuilder<PondDetail>()
                .WithPredicate(x => x.UserPondId == userPondId)
                .Build());

            if ((koiDetails == null || !koiDetails.Any()) && (pondDetails == null || !pondDetails.Any()))
            {
                return new ResponseApiDTO
                {
                    IsSuccess = false,
                    Message = "Không tìm thấy chi tiết Koi hoặc hồ cho UserPond này.",
                    Result = null
                };
            }

            // Map entities to DTOs
            var koiDetailDtos = _mapper.Map<IEnumerable<KoiDetailDTO>>(koiDetails);
            var pondDetailDtos = _mapper.Map<IEnumerable<PondDetailDTO>>(pondDetails);

            // Return result as a combined response
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
                Message = "Có lỗi xảy ra khi lấy chi tiết Koi và hồ.",
                Result = null
            };
        }
    }




}