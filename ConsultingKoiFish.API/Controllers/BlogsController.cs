using ConsultingKoiFish.BLL.DTOs;
using ConsultingKoiFish.BLL.DTOs.BlogDTOs;
using ConsultingKoiFish.BLL.DTOs.ImageDTOs;
using ConsultingKoiFish.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ConsultingKoiFish.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogsController : BaseAPIController
    {
        private readonly IBlogService _blogService;

        public BlogsController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        [Authorize]
        [HttpPost]
        [Route("create-update-blogs")]
        public async Task<IActionResult> CreateUpdateBlog(BlogRequestDTO dto)
        {
            try
            {
                var response = await _blogService.CraeteUpdateBlog(dto, UserId);
                if (!response.IsSuccess) return SaveError(response);
                return SaveSuccess(response);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút nữa.");
            }
        }
        
        [HttpGet]
        [Route("get-all-blogs/{pageIndex}/{pageSize}")]
        public async Task<IActionResult> GetAllBlogs([FromRoute] int pageIndex, [FromRoute] int pageSize)
        {
            try
            {
                if (pageIndex <= 0)
                {
                    return GetError("Page Index phải là số nguyên dương.");
                }

                if (pageSize <= 0)
                {
                    return GetError("Page Size phải là số nguyên dương.");
                }

                var data = await _blogService.GetAllBlogs( pageIndex, pageSize);
                var response = new PagingDTO<BlogViewDTO>(data);
                if (response == null) return GetError();
                return GetSuccess(response);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút nữa.");
            }
        }
        
        [Authorize]
        [HttpGet]
        [Route("get-all-blogs-by-userId/{pageIndex}/{pageSize}")]
        public async Task<IActionResult> GetAllBlogsByUserId([FromRoute] int pageIndex, [FromRoute] int pageSize)
        {
            try
            {
                if (pageIndex <= 0)
                {
                    return GetError("Page Index phải là số nguyên dương.");
                }

                if (pageSize <= 0)
                {
                    return GetError("Page Size phải là số nguyên dương.");
                }

                var data = await _blogService.GetAllBlogsByUserId(UserId, pageIndex, pageSize);
                var response = new PagingDTO<BlogViewDTO>(data);
                if (response == null) return GetError();
                return GetSuccess(response);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút nữa.");
            }
        }
        
        [HttpGet]
        [Route("filter-all-blogs-by-title/{pageIndex}/{pageSize}")]
        public async Task<IActionResult> GetAllBlogsByTitle(string? title, [FromRoute] int pageIndex, [FromRoute] int pageSize)
        {
            try
            {
                if (pageIndex <= 0)
                {
                    return GetError("Page Index phải là số nguyên dương.");
                }

                if (pageSize <= 0)
                {
                    return GetError("Page Size phải là số nguyên dương.");
                }

                var data = await _blogService.GetAllBlogsByTitle(title, pageIndex, pageSize);
                var response = new PagingDTO<BlogViewDTO>(data);
                if (response == null) return GetError();
                return GetSuccess(response);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút nữa.");
            }
        }
        
        [HttpGet]
        [Route("get-blog-by-id/{id}")]
        public async Task<IActionResult> GetBlogById([FromRoute] int id)
        {
            try
            {
                var response = await _blogService.GetBlogById(id);
                if (response == null) return GetError("Blog này không tồn tại.");
                return GetSuccess(response);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút nữa.");
            }
        }
        
        [Authorize]
        [HttpPost]
        [Route("delete-blog/{id}")]
        public async Task<IActionResult> DeleteImage(int id)
        {
            try
            {
                var response = await _blogService.DeleteBlog(id, UserId);
                if (!response.IsSuccess) return SaveError(response);
                return SaveSuccess(response);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút nữa.");
            }
        }
    }
}
