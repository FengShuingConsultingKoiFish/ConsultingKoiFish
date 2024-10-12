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



		#region Common

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

		[HttpPost]
        [Route("get-all-blogs")]
        public async Task<IActionResult> GetAllBlogs(BlogGetListDTO dto)
        {
            try
            {
				if (dto.PageIndex <= 0)
				{
					ModelState.AddModelError("PageIndex", "PageIndex phải là số nguyên dương");
					return ModelInvalid();
				}

				if (dto.PageSize <= 0)
				{
					ModelState.AddModelError("PageSize", "PageIndex phải là số nguyên dương");
					return ModelInvalid();
				}

				var data = await _blogService.GetAllBlogs(dto);
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
		[Route("get-blog-by-id/{id}/{orderComment}")]
		public async Task<IActionResult> GetBlogById([FromRoute] int id, [FromRoute] OrderComment? orderComment)
		{
			try
			{
				var response = await _blogService.GetBlogById(id, orderComment);
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

		#region User

		[Authorize(Roles = "Member")]
        [HttpPost]
        [Route("get-all-blogs-for-user")]
        public async Task<IActionResult> GetAllBlogsByUserId(BlogGetListDTO dto)
        {
            try
            {
                if (dto.PageIndex <= 0)
                {
                    ModelState.AddModelError("PageIndex", "PageIndex phải là số nguyên dương");
                    return ModelInvalid();
                }

                if (dto.PageSize <= 0)
                {
					ModelState.AddModelError("PageSize", "PageIndex phải là số nguyên dương");
					return ModelInvalid();
				}

                var data = await _blogService.GetAllBlogsByUserId(UserId, dto);
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
        
        
        #region Admin
        
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("get-all-blogs-for-admin")]
        public async Task<IActionResult> GetAllBlogsForAdmin(BlogGetListDTO dto)
        {
            try
            {
				if (dto.PageIndex <= 0)
				{
					ModelState.AddModelError("PageIndex", "PageIndex phải là số nguyên dương");
					return ModelInvalid();
				}

				if (dto.PageSize <= 0)
				{
					ModelState.AddModelError("PageSize", "PageIndex phải là số nguyên dương");
					return ModelInvalid();
				}

				var data = await _blogService.GetAllBlogsForAdmin(dto);
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

        
    }
}