using ConsultingKoiFish.BLL.DTOs;
using ConsultingKoiFish.BLL.DTOs.BlogDTOs;
using ConsultingKoiFish.BLL.DTOs.BlogImageDTOs;
using ConsultingKoiFish.BLL.DTOs.ImageDTOs;
using ConsultingKoiFish.BLL.Services.Interfaces;
using ConsultingKoiFish.DAL.Enums;
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
                if (!ModelState.IsValid) return ModelInvalid();
                var response = await _blogService.CreateUpdateBlog(dto, UserId);
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

        #region Common
        
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

                var data = await _blogService.GetAllBlogs(pageIndex, pageSize);
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
        public async Task<IActionResult> GetAllBlogsByTitle(string? title, [FromRoute] int pageIndex,
            [FromRoute] int pageSize)
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
        
        #endregion
        
        #region User
        
        [Authorize(Roles = "Member")]
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

        [Authorize(Roles = "Member")]
        [HttpGet]
        [Route("filter-all-blogs-by-userId-with-status/{pageIndex}/{pageSize}/{status}")]
        public async Task<IActionResult> GetAllBlogsByUserIdWithStatus([FromRoute] BlogStatus? status,
            [FromRoute] int pageIndex, [FromRoute] int pageSize)
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

                var data = await _blogService.GetAllBlogsByUserIdWithStatus(status, UserId, pageIndex, pageSize);
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
        [HttpPost]
        [Route("add-images-to-blogs")]
        public async Task<IActionResult> AddImagesToBlog(BlogImageRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid) return ModelInvalid();
                var response = await _blogService.AddImagesToBlog(dto);
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
        
        
        [Authorize]
        [HttpPost]
        [Route("delete-images-from-blog")]
        public async Task<IActionResult> DeleteImagesFromBlog(BlogImageDeleteDTO dto)
        {
            try
            {
                if (!ModelState.IsValid) return ModelInvalid();
                var response = await _blogService.DeleteImagesFromBlog(dto);
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
        
        #endregion
        
        
        #region Admin
        
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("get-all-blogs-for-admin/{pageIndex}/{pageSize}")]
        public async Task<IActionResult> GetAllBlogsForAdmin([FromRoute] int pageIndex, [FromRoute] int pageSize)
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

                var data = await _blogService.GetAllBlogsForAdmin(pageIndex, pageSize);
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
        
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("filter-all-blogs-for-admin-with-status/{pageIndex}/{pageSize}/{status}")]
        public async Task<IActionResult> GetAllBlogsForAdminWithStatus([FromRoute] BlogStatus? status,
            [FromRoute] int pageIndex, [FromRoute] int pageSize)
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

                var data = await _blogService.GetAllBlogsForAdminWithStatus(status, pageIndex, pageSize);
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
        
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("filter-all-blogs-for-admin-by-title/{pageIndex}/{pageSize}/{title}")]
        public async Task<IActionResult> GetAllBlogsForAdminWithTitle([FromRoute]string? title, [FromRoute] int pageIndex,
            [FromRoute] int pageSize)
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

                var data = await _blogService.GetAllBlogsForAdminWithTitle(title, pageIndex, pageSize);
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
        
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("update-status-blogs")]
        public async Task<IActionResult> UpdateStatusBlog(BlogUpdateStatusDTO dto)
        {
            try
            {
                if (!ModelState.IsValid) return ModelInvalid();
                if (!dto.IsStatusValid())
                {
                    ModelState.AddModelError("Status", "Status không hợp lệ");
                }
                var response = await _blogService.UpdateStatusBlog(dto);
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
        
        #endregion

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

        [Authorize(Roles = "Member")]
        [HttpPost]
        [Route("delete-blog/{id}")]
        public async Task<IActionResult> DeleteBlog(int id)
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