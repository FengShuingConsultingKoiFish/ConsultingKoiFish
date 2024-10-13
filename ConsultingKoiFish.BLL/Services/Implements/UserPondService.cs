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
            // Lấy repository của UserPond
            var repo = _unitOfWork.GetRepo<UserPond>();

            // Kiểm tra xem UserPond có tồn tại không
            var userPond = await repo.GetSingleAsync(new QueryBuilder<UserPond>()
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

            // Thực hiện xóa
            await repo.DeleteAsync(userPond);

            // Lưu thay đổi
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