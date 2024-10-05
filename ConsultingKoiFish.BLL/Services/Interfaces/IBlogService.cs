using ConsultingKoiFish.BLL.DTOs.BlogDTOs;
using ConsultingKoiFish.BLL.DTOs.Response;
using ConsultingKoiFish.DAL.Paging;

namespace ConsultingKoiFish.BLL.Services.Interfaces;

public interface IBlogService
{
    Task<BaseResponse> CraeteUpdateBlog(BlogRequestDTO dto, string userId);
    Task<PaginatedList<BlogViewDTO>> GetAllBlogs(int pageIndex, int pageSize);
    Task<BlogViewDTO> GetBlogById(int id);
}