using AutoMapper;
using ConsultingKoiFish.BLL.DTOs.Response;
using ConsultingKoiFish.BLL.DTOs.ZodiacDTO;
using ConsultingKoiFish.BLL.Services.Interfaces;
using ConsultingKoiFish.DAL.Entities;
using ConsultingKoiFish.DAL.Queries;
using ConsultingKoiFish.DAL.UnitOfWork;

namespace ConsultingKoiFish.BLL.Services.Implements;

public class ZodiacService : IZodiacService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ZodiacService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        this._unitOfWork = unitOfWork;
        this._mapper = mapper;
    }
    public async Task<BaseResponse> AddZodiac(ZodiacRequestDTO zodiacRequestDto)
    {
        try
        {
            var repo = _unitOfWork.GetRepo<Zodiac>();
            var zodiacDTO = new Zodiac()
            {
                ZodiacName = zodiacRequestDto.ZodiacName
            };
            await repo.CreateAsync(zodiacDTO);
            return new BaseResponse { IsSuccess = true, Message = "Thêm mệnh thành công thành công" };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<BaseResponse> UpdateZodiac(ZodiacRequestDTO zodiacRequestDto, int zodiacID)
    {
        try
        {
            var repo = _unitOfWork.GetRepo<Zodiac>();
            await _unitOfWork.BeginTransactionAsync();
            var any = await repo.AnyAsync(new QueryBuilder<Zodiac>()
                .WithPredicate(x => x.Id.Equals(zodiacID))
                .Build());
            if(any)
            {
                var userDetail = await repo.GetSingleAsync(new QueryBuilder<Zodiac>()
                    .WithPredicate(x => x.Id.Equals(zodiacID))
                    .Build());
                if(zodiacID != userDetail.Id) return new BaseResponse { IsSuccess = false, Message = "Người dùng không khớp."};

                var updateUserDetail = _mapper.Map(zodiacRequestDto, userDetail);
                await repo.UpdateAsync(updateUserDetail);
            }
            var saver = await _unitOfWork.SaveAsync();
            await _unitOfWork.CommitTransactionAsync();
            if (!saver) return new BaseResponse { IsSuccess = false, Message = "Lưu dữ liệu thất bại" };
            return new BaseResponse { IsSuccess = true, Message = "Lưu dữ liệu thành công" };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<ResponseApiDTO> GetAllZodiacs()
    {
        try
        {
            // Sử dụng QueryBuilder để xây dựng truy vấn
            var queryBuilder = new QueryBuilder<Zodiac>()
                .WithOrderBy(q => q.OrderBy(z => z.ZodiacName))  // Sắp xếp theo tên Zodiac
                .WithTracking(false);  // Không cần theo dõi các thực thể này

            // Lấy tất cả các bản ghi sử dụng GetAllAsync và QueryOptions từ QueryBuilder
            var repo = _unitOfWork.GetRepo<Zodiac>();
            var zodiacs = await repo.GetAllAsync(queryBuilder.Build());

            // Map kết quả từ thực thể sang DTO
            var zodiacDTOs = _mapper.Map<List<Zodiac>>(zodiacs);
            return new ResponseApiDTO { IsSuccess = true, Result = zodiacDTOs };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new ResponseApiDTO { IsSuccess = false, Message = "Không thể lấy danh sách cung hoàng đạo." };
        }
    }



    public async Task<BaseResponse> DeleteZodiac(int zodiacID)
    {
        try
        {
            var repo = _unitOfWork.GetRepo<Zodiac>();
            var zodiac = await repo.GetSingleAsync(new QueryBuilder<Zodiac>()
                .WithPredicate(x => x.Id == zodiacID)
                .Build());

            if (zodiac == null)
            {
                return new BaseResponse { IsSuccess = false, Message = "Không tìm thấy cung hoàng đạo." };
            }

            await repo.DeleteAsync(zodiac);
            var saver = await _unitOfWork.SaveAsync();

            if (!saver) 
                return new BaseResponse { IsSuccess = false, Message = "Xóa thất bại." };

            return new BaseResponse { IsSuccess = true, Message = "Xóa thành công." };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new BaseResponse { IsSuccess = false, Message = "Có lỗi xảy ra khi xóa cung hoàng đạo." };
        }
    }
    public async Task<ResponseApiDTO> GetZodiacByBirthDate(DateTime birthDate, string userId)
{
    try
    {
        var year = birthDate.Year;
        
        // Tính Thiên Can và Địa Chi
        int thienCan = year % 10;
        int diaChi = year % 12;

        // Bảng tra mệnh theo Can Chi
        string[,] canChiToMenh = {
            { "Kim", "Kim", "Mộc", "Mộc", "Thổ", "Thổ", "Hỏa", "Hỏa", "Thổ", "Thổ" }, // Tý
            { "Kim", "Kim", "Mộc", "Mộc", "Thổ", "Thổ", "Hỏa", "Hỏa", "Thổ", "Thổ" }, // Sửu
            { "Mộc", "Mộc", "Hỏa", "Hỏa", "Thổ", "Thổ", "Kim", "Kim", "Thổ", "Thổ" }, // Dần
            { "Mộc", "Mộc", "Hỏa", "Hỏa", "Thổ", "Thổ", "Kim", "Kim", "Thổ", "Thổ" }, // Mão
            { "Thổ", "Thổ", "Kim", "Kim", "Thổ", "Thổ", "Mộc", "Mộc", "Thủy", "Thủy" }, // Thìn
            { "Thổ", "Thổ", "Kim", "Kim", "Thổ", "Thổ", "Mộc", "Mộc", "Thủy", "Thủy" }, // Tỵ
            { "Hỏa", "Hỏa", "Thổ", "Thổ", "Mộc", "Mộc", "Thủy", "Thủy", "Thổ", "Thổ" }, // Ngọ
            { "Hỏa", "Hỏa", "Thổ", "Thổ", "Mộc", "Mộc", "Thủy", "Thủy", "Thổ", "Thổ" }, // Mùi
            { "Thổ", "Thổ", "Kim", "Kim", "Thủy", "Thủy", "Mộc", "Mộc", "Thổ", "Thổ" }, // Thân
            { "Thổ", "Thổ", "Kim", "Kim", "Thủy", "Thủy", "Mộc", "Mộc", "Thổ", "Thổ" }, // Dậu
            { "Thổ", "Thổ", "Hỏa", "Hỏa", "Kim", "Kim", "Mộc", "Mộc", "Thủy", "Thủy" }, // Tuất
            { "Thổ", "Thổ", "Hỏa", "Hỏa", "Kim", "Kim", "Mộc", "Mộc", "Thủy", "Thủy" }  // Hợi
        };

        // Lấy mệnh của người dùng
        string menh = canChiToMenh[diaChi, thienCan];

        // Bước 1: Lấy Zodiac dựa trên mệnh đã tính toán
        var zodiacRepo = _unitOfWork.GetRepo<Zodiac>();
        var zodiac = await zodiacRepo.GetSingleAsync(new QueryBuilder<Zodiac>()
            .WithPredicate(z => z.ZodiacName.Equals(menh))
            .Build());

        if (zodiac == null)
        {
            return new ResponseApiDTO { IsSuccess = false, Message = "Không tìm thấy cung hoàng đạo cho mệnh này." };
        }

        // Bước 2: Thêm vào bảng UserZodiac với userId và ZodiacId
        var userZodiacRepo = _unitOfWork.GetRepo<UserZodiac>();
        var userZodiac = new UserZodiac
        {
            UserId = userId,
            ZodiacId = zodiac.Id
        };

        await userZodiacRepo.CreateAsync(userZodiac);

        // Lưu thay đổi
        var saveSuccess = await _unitOfWork.SaveAsync();
        if (!saveSuccess)
        {
            return new ResponseApiDTO { IsSuccess = false, Message = "Không thể lưu thông tin mệnh của người dùng." };
        }

        // Trả về mệnh của người dùng và trạng thái thành công
        return new ResponseApiDTO { IsSuccess = true, Result = menh };
    }
    catch (Exception ex)
    {
        return new ResponseApiDTO { IsSuccess = false, Message = "Có lỗi xảy ra khi tính mệnh và lưu thông tin: " + ex.Message };
    }
}


}