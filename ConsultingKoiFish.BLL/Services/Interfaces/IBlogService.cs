using ConsultingKoiFish.BLL.DTOs.BlogDTOs;
using ConsultingKoiFish.BLL.DTOs.Response;

namespace ConsultingKoiFish.BLL.Services.Interfaces;

public interface IBlogService
{
    Task<BaseResponse> CraeteUpdateBlog(BlogRequestDTO dto, string userId);
}