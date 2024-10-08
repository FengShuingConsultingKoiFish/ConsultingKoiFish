using System.Net;
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
            await _unitOfWork.SaveChangesAsync();
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
            
            var zodiacExists = await repo.AnyAsync(new QueryBuilder<Zodiac>()
                .WithPredicate(x => x.Id.Equals(zodiacID))
                .Build());

            if (!zodiacExists)
            {
                return new BaseResponse { IsSuccess = false, Message = "Zodiac không tồn tại." };
            }
            
            var zodiacDetail = await repo.GetSingleAsync(new QueryBuilder<Zodiac>()
                .WithPredicate(x => x.Id.Equals(zodiacID))
                .Build());
            
            zodiacDetail.ZodiacName = zodiacRequestDto.ZodiacName;
            
            await repo.UpdateAsync(zodiacDetail);
            
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



    public async Task<ResponseApiDTO> GetAllZodiacs()
    {
        try
        {
            var queryBuilder = new QueryBuilder<Zodiac>()
                .WithOrderBy(q => q.OrderBy(z => z.ZodiacName))  
                .WithTracking(false);  
            
            var repo = _unitOfWork.GetRepo<Zodiac>();
            var zodiacs = await repo.GetAllAsync(queryBuilder.Build());
            
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
        
        // Tính Thiên Can và Địa Chi chính xác
        int thienCan = (year + 6) % 10;
        int diaChi = (year + 8) % 12;
    
        // Bảng Thiên Can
        string[] thienCanList = { "Giáp", "Ất", "Bính", "Đinh", "Mậu", "Kỷ", "Canh", "Tân", "Nhâm", "Quý" };
        
        // Bảng Địa Chi
        string[] diaChiList = { "Tý", "Sửu", "Dần", "Mão", "Thìn", "Tỵ", "Ngọ", "Mùi", "Thân", "Dậu", "Tuất", "Hợi" };
        
        // Bảng mệnh ngũ hành cho 60 năm (chu kỳ Lục Thập Hoa Giáp)
        var canChiToMenh = new Dictionary<string, string>
        {
            { "Giáp Tý", "Kim" },
            { "Ất Sửu", "Kim" },
            { "Bính Dần", "Hỏa" },
            { "Đinh Mão", "Hỏa" },
            { "Mậu Thìn", "Mộc" },
            { "Kỷ Tỵ", "Mộc" },
            { "Canh Ngọ", "Thổ" },
            { "Tân Mùi", "Thổ" },
            { "Nhâm Thân", "Kim" },
            { "Quý Dậu", "Kim" },
            { "Giáp Tuất", "Hỏa" },
            { "Ất Hợi", "Hỏa" },
            { "Bính Tý", "Thủy" },
            { "Đinh Sửu", "Thủy" },
            { "Mậu Dần", "Thổ" },
            { "Kỷ Mão", "Thổ" },
            { "Canh Thìn", "Kim" },
            { "Tân Tỵ", "Kim" },
            { "Nhâm Ngọ", "Mộc" },
            { "Quý Mùi", "Mộc" },
            { "Giáp Thân", "Thủy" },
            { "Ất Dậu", "Thủy" },
            { "Bính Tuất", "Thổ" },
            { "Đinh Hợi", "Thổ" },
            { "Mậu Tý", "Hỏa" },
            { "Kỷ Sửu", "Hỏa" },
            { "Canh Dần", "Thổ" },
            { "Tân Mão", "Thổ" },
            { "Nhâm Thìn", "Thủy" },
            { "Quý Tỵ", "Thủy" },
            { "Giáp Ngọ", "Kim" },
            { "Ất Mùi", "Kim" },
            { "Bính Thân", "Hỏa" },
            { "Đinh Dậu", "Hỏa" },
            { "Mậu Tuất", "Mộc" },
            { "Kỷ Hợi", "Mộc" },
            { "Canh Tý", "Thổ" },
            { "Tân Sửu", "Thổ" },
            { "Nhâm Dần", "Kim" },
            { "Quý Mão", "Kim" },
            { "Giáp Thìn", "Hỏa" },
            { "Ất Tỵ", "Hỏa" },
            { "Bính Ngọ", "Thủy" },
            { "Đinh Mùi", "Thủy" },
            { "Mậu Thân", "Thổ" },
            { "Kỷ Dậu", "Thổ" },
            { "Canh Tuất", "Kim" },
            { "Tân Hợi", "Kim" },
            { "Nhâm Tý", "Mộc" },
            { "Quý Sửu", "Mộc" },
            { "Giáp Dần", "Thủy" },
            { "Ất Mão", "Thủy" },
            { "Bính Thìn", "Thổ" },
            { "Đinh Tỵ", "Thổ" },
            { "Mậu Ngọ", "Hỏa" },
            { "Kỷ Mùi", "Hỏa" },
            { "Canh Thân", "Mộc" },
            { "Tân Dậu", "Mộc" },
            { "Nhâm Tuất", "Thủy" },
            { "Quý Hợi", "Thủy" },
        };
        
        // Lấy Thiên Can và Địa Chi
        string can = thienCanList[thienCan];
        string chi = diaChiList[diaChi];
        string canChi = $"{can} {chi}";
    
        // Lấy mệnh từ bảng canChiToMenh
        string menh;
        if (canChiToMenh.TryGetValue(canChi, out menh))
        {
            return new ResponseApiDTO
            {
                StatusCode = HttpStatusCode.OK,
                IsSuccess = true,
                Result = menh,
                Message = $"Năm {year} là năm {canChi}, mệnh {menh}."
            };
        }
        else
        {
            return new ResponseApiDTO
            {
                IsSuccess = false,
                Message = $"Không tìm thấy mệnh cho năm {year} ({canChi})."
            };
        }
    }
    catch (Exception ex)
    {
        return new ResponseApiDTO
        {
            IsSuccess = false,
            Message = "Có lỗi xảy ra khi tính mệnh và lưu thông tin: " + ex.Message
        };
    }
}



}