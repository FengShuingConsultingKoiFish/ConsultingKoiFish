using ConsultingKoiFish.BLL.DTOs.BlogDTOs;
using ConsultingKoiFish.BLL.DTOs.Response;
using ConsultingKoiFish.DAL.Paging;

namespace ConsultingKoiFish.BLL.Services.Interfaces;

public interface IBlogService
{
    Task<BaseResponse> CraeteUpdateBlog(BlogRequestDTO dto, string userId);
    Task<PaginatedList<BlogViewDTO>> GetAllBlogs(int pageIndex, int pageSize);
    Task<PaginatedList<BlogViewDTO>> GetAllBlogsByUserId(string userId, int pageIndex, int pageSize);
    Task<PaginatedList<BlogViewDTO>> GetAllBlogsByTitle(string? title, int pageIndex, int pageSize);
    Task<BlogViewDTO> GetBlogById(int id);
    Task<BaseResponse> DeleteBlog(int id, string userId);
}
