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
    Task<PaginatedList<BlogViewDTO>> GetAllBlogs(BlogGetListDTO dto);
    Task<PaginatedList<BlogViewDTO>> GetAllBlogsByUserId(string userId, BlogGetListDTO dto);
    Task<PaginatedList<BlogViewDTO>> GetAllBlogsForAdmin(BlogGetListDTO dto);
    Task<BlogViewDTO> GetBlogById(int id, OrderComment? orderComment, OrderImage? orderImage);
    Task<BaseResponse> DeleteBlog(int id, string userId);
}
