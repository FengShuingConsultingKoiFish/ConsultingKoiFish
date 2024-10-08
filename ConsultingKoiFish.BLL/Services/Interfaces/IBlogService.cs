using ConsultingKoiFish.BLL.DTOs.BlogDTOs;
using ConsultingKoiFish.BLL.DTOs.BlogImageDTOs;
using ConsultingKoiFish.BLL.DTOs.Response;
using ConsultingKoiFish.DAL.Enums;
using ConsultingKoiFish.DAL.Paging;

namespace ConsultingKoiFish.BLL.Services.Interfaces;

public interface IBlogService
{
    Task<BaseResponse> CreateUpdateBlog(BlogRequestDTO dto, string userId);
    Task<BaseResponse> UpdateStatusBlog(BlogUpdateStatusDTO dto);
    Task<BaseResponse> AddImagesToBlog(BlogImageRequestDTO dto);
    Task<BaseResponse> DeleteImagesFromBlog(BlogImageDeleteDTO dto);
    Task<PaginatedList<BlogViewDTO>> GetAllBlogs(int pageIndex, int pageSize);
    Task<PaginatedList<BlogViewDTO>> GetAllBlogsByUserId(string userId, int pageIndex, int pageSize);
    Task<PaginatedList<BlogViewDTO>> GetAllBlogsByUserIdWithStatus(BlogStatus? status, string userId, int pageIndex, int pageSize);
    Task<PaginatedList<BlogViewDTO>> GetAllBlogsByTitle(string? title, int pageIndex, int pageSize);
    Task<PaginatedList<BlogViewDTO>> GetAllBlogsForAdmin(int pageIndex, int pageSize);
    Task<PaginatedList<BlogViewDTO>> GetAllBlogsForAdminWithStatus(BlogStatus? status, int pageIndex, int pageSize);
    Task<PaginatedList<BlogViewDTO>> GetAllBlogsForAdminWithTitle(string? title, int pageIndex, int pageSize);
    Task<BlogViewDTO> GetBlogById(int id);
    Task<BaseResponse> DeleteBlog(int id, string userId);
}
