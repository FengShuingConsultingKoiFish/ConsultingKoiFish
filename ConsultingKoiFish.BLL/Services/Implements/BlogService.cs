using AutoMapper;
using ConsultingKoiFish.BLL.DTOs.BlogDTOs;
using ConsultingKoiFish.BLL.DTOs.BlogImageDTOs;
using ConsultingKoiFish.BLL.DTOs.ImageDTOs;
using ConsultingKoiFish.BLL.DTOs.Response;
using ConsultingKoiFish.BLL.Services.Interfaces;
using ConsultingKoiFish.DAL.Entities;
using ConsultingKoiFish.DAL.Enums;
using ConsultingKoiFish.DAL.Paging;
using ConsultingKoiFish.DAL.Queries;
using ConsultingKoiFish.DAL.UnitOfWork;

namespace ConsultingKoiFish.BLL.Services.Implements;

public class BlogService : IBlogService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public BlogService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<BaseResponse> CreateUpdateBlog(BlogRequestDTO dto, string userId)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            var repo = _unitOfWork.GetRepo<Blog>();
            var blogImageRepo = _unitOfWork.GetRepo<BlogImage>();
            var any = await repo.AnyAsync(new QueryBuilder<Blog>()
                .WithPredicate(x => x.Id == dto.Id)
                .Build());
            if (any)
            {
                var blog = await repo.GetSingleAsync(new QueryBuilder<Blog>()
                    .WithPredicate(x => x.Id == dto.Id)
                    .WithTracking(false)
                    .Build());
                if (!blog.UserId.Equals(userId))
                    return new BaseResponse
                    {
                        IsSuccess = false,
                        Message = "Blog không thuộc sở hữu người dùng."
                    };
                var updateBlogDto = new BlogUpdateDTO { Content = dto.Content, Title = dto.Title };
                var updateBlog = _mapper.Map(updateBlogDto, blog);
                await repo.UpdateAsync(updateBlog);
            }
            else
            {
                var createdBlogDto = new BlogCreateDTO
                {
                    Content = dto.Content,
                    Title = dto.Title
                };
                var createdBlog = _mapper.Map<Blog>(createdBlogDto);
                createdBlog.UserId = userId;
                createdBlog.IsActive = true;
                createdBlog.CreatedDate = DateTime.Now;
                createdBlog.Status = (int)BlogStatus.Pending;
                await repo.CreateAsync(createdBlog);
                await _unitOfWork.SaveChangesAsync();

                if (dto.ImageIds != null || dto.ImageIds.Any())
                {
                    var createdBlogImageDTOs = new List<BlogImageCreateDTO>();
                    foreach (var image in dto.ImageIds)
                    {
						var existedImage = await _unitOfWork.GetRepo<Image>().GetSingleAsync(new QueryBuilder<Image>()
																							.WithPredicate(x => x.Id == image)
																							.WithTracking(false)
																							.Build());
						if (existedImage == null) return new BaseResponse { IsSuccess = false, Message = $"Ảnh {image} không tồn tại." };
						var createdBlogImageDto = new BlogImageCreateDTO
                        {
                            BlogId = createdBlog.Id,
                            ImageId = image
                        };
                        createdBlogImageDTOs.Add(createdBlogImageDto);
                    }
                    var createdBlogImages = _mapper.Map<List<BlogImage>>(createdBlogImageDTOs);
                    await blogImageRepo.CreateAllAsync(createdBlogImages);
                }
            }

            var saver = await _unitOfWork.SaveAsync();
            await _unitOfWork.CommitTransactionAsync();
            if (!saver)
            {
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = "Lưu dữ liệu không thành công."
                };
            }

            return new BaseResponse { IsSuccess = true, Message = "Lưu dữ liệu thành công." };
        }
        catch (Exception)
        {
            await _unitOfWork.RollBackAsync();
            throw;
        }
    }

    //thằng này get hết blog đã được chấp thuận của hệ thống
    public async Task<PaginatedList<BlogViewDTO>> GetAllBlogs(int pageIndex, int pageSize)
    {
        var repo = _unitOfWork.GetRepo<Blog>();
        var imageRepo = _unitOfWork.GetRepo<Image>();
        var loadedRecords = repo.Get(new QueryBuilder<Blog>()
            .WithPredicate(x => x.IsActive == true && x.Status == (int)BlogStatus.Approved)
            .WithTracking(false)
            .WithInclude(x => x.User, r => r.BlogImages)
            .Build());
        var pagedRecords = await PaginatedList<Blog>.CreateAsync(loadedRecords, pageIndex, pageSize);
        var response = new List<BlogViewDTO>();
        foreach (var blog in pagedRecords)
        {
            var blogImageViewDtos = await ConvertToImageViews(blog.BlogImages);
            var childResponse = new BlogViewDTO(blog, blogImageViewDtos);
            response.Add(childResponse);
        }
        return new PaginatedList<BlogViewDTO>(response, pagedRecords.TotalItems, pageIndex, pageSize);
    }

    //thằng này get hết blog của th user
    public async Task<PaginatedList<BlogViewDTO>> GetAllBlogsByUserId(string userId, int pageIndex, int pageSize)
    {
        var repo = _unitOfWork.GetRepo<Blog>();
        var imageRepo = _unitOfWork.GetRepo<Image>();
        var loadedRecords = repo.Get(new QueryBuilder<Blog>()
            .WithPredicate(x => x.IsActive == true && x.UserId.Equals(userId))
            .WithTracking(false)
            .WithInclude(x => x.User, r => r.BlogImages)
            .Build());
        var pagedRecords = await PaginatedList<Blog>.CreateAsync(loadedRecords, pageIndex, pageSize);
        var response = new List<BlogViewDTO>();
        foreach (var blog in pagedRecords)
        {
            var blogImageViewDtos = await ConvertToImageViews(blog.BlogImages);
            var childResponse = new BlogViewDTO(blog, blogImageViewDtos);
            response.Add(childResponse);
        }
        return new PaginatedList<BlogViewDTO>(response, pagedRecords.TotalItems, pageIndex, pageSize);
    }

    //thằng này get hết blog của thằng user nhưng filter với status
    public async Task<PaginatedList<BlogViewDTO>> GetAllBlogsByUserIdWithStatus(BlogStatus? status, string userId, int pageIndex, int pageSize)
    {
        var repo = _unitOfWork.GetRepo<Blog>();
        var imageRepo = _unitOfWork.GetRepo<Image>();
        var loadedRecords = repo.Get(new QueryBuilder<Blog>()
            .WithPredicate(x => x.IsActive == true && x.UserId.Equals(userId))
            .WithTracking(false)
            .WithInclude(x => x.User, r => r.BlogImages)
            .Build());
        if (status.HasValue)
        {
            loadedRecords = loadedRecords.Where(x => x.Status == (int)status);
        }
        var pagedRecords = await PaginatedList<Blog>.CreateAsync(loadedRecords, pageIndex, pageSize);
        var response = new List<BlogViewDTO>();
        foreach (var blog in pagedRecords)
        {
            var blogImageViewDtos = await ConvertToImageViews(blog.BlogImages);
            var childResponse = new BlogViewDTO(blog, blogImageViewDtos);
            response.Add(childResponse);
        }
        return new PaginatedList<BlogViewDTO>(response, pagedRecords.TotalItems, pageIndex, pageSize);
    }

    //thằng này để tra cứu blog theo title ở homepage
    public async Task<PaginatedList<BlogViewDTO>> GetAllBlogsByTitle(string? title, int pageIndex, int pageSize)
    {
        var repo = _unitOfWork.GetRepo<Blog>();
        var imageRepo = _unitOfWork.GetRepo<Image>();
        var loadedRecords = repo.Get(new QueryBuilder<Blog>()
            .WithPredicate(x => x.IsActive == true && x.Status == (int)BlogStatus.Approved)
            .WithTracking(false)
            .WithInclude(x => x.User, r => r.BlogImages)
            .Build());
        if (!string.IsNullOrEmpty(title))
        {
            loadedRecords = loadedRecords.Where(x => x.Title.Contains(title));
        }
        var pagedRecords = await PaginatedList<Blog>.CreateAsync(loadedRecords, pageIndex, pageSize);
        var response = new List<BlogViewDTO>();
        foreach (var blog in pagedRecords)
        {
            var blogImageViewDtos = await ConvertToImageViews(blog.BlogImages);
            var childResponse = new BlogViewDTO(blog, blogImageViewDtos);
            response.Add(childResponse);
        }
        return new PaginatedList<BlogViewDTO>(response, pagedRecords.TotalItems, pageIndex, pageSize);
    }

    public async Task<PaginatedList<BlogViewDTO>> GetAllBlogsForAdmin(int pageIndex, int pageSize)
    {
        var repo = _unitOfWork.GetRepo<Blog>();
        var imageRepo = _unitOfWork.GetRepo<Image>();
        var loadedRecords = repo.Get(new QueryBuilder<Blog>()
            .WithPredicate(x => x.IsActive == true)
            .WithTracking(false)
            .WithInclude(x => x.User, r => r.BlogImages)
            .Build());
        var pagedRecords = await PaginatedList<Blog>.CreateAsync(loadedRecords, pageIndex, pageSize);
        var response = new List<BlogViewDTO>();
        foreach (var blog in pagedRecords)
        {
            var blogImageViewDtos = await ConvertToImageViews(blog.BlogImages);
            var childResponse = new BlogViewDTO(blog, blogImageViewDtos);
            response.Add(childResponse);
        }
        return new PaginatedList<BlogViewDTO>(response, pagedRecords.TotalItems, pageIndex, pageSize);
    }

    public async Task<PaginatedList<BlogViewDTO>> GetAllBlogsForAdminWithStatus(BlogStatus? status, int pageIndex, int pageSize)
    {
        var repo = _unitOfWork.GetRepo<Blog>();
        var imageRepo = _unitOfWork.GetRepo<Image>();
        var loadedRecords = repo.Get(new QueryBuilder<Blog>()
            .WithPredicate(x => x.IsActive == true)
            .WithTracking(false)
            .WithInclude(x => x.User, r => r.BlogImages)
            .Build());
        if (status.HasValue)
        {
            loadedRecords = loadedRecords.Where(x => x.Status == (int)status);
        }
        var pagedRecords = await PaginatedList<Blog>.CreateAsync(loadedRecords, pageIndex, pageSize);
        var response = new List<BlogViewDTO>();
        foreach (var blog in pagedRecords)
        {
            var blogImageViewDtos = await ConvertToImageViews(blog.BlogImages);
            var childResponse = new BlogViewDTO(blog, blogImageViewDtos);
            response.Add(childResponse);
        }
        return new PaginatedList<BlogViewDTO>(response, pagedRecords.TotalItems, pageIndex, pageSize);
    }

    public async Task<PaginatedList<BlogViewDTO>> GetAllBlogsForAdminWithTitle(string? title, int pageIndex, int pageSize)
    {
        var repo = _unitOfWork.GetRepo<Blog>();
        var imageRepo = _unitOfWork.GetRepo<Image>();
        var loadedRecords = repo.Get(new QueryBuilder<Blog>()
            .WithPredicate(x => x.IsActive == true)
            .WithTracking(false)
            .WithInclude(x => x.User, r => r.BlogImages)
            .Build());
        if (!string.IsNullOrEmpty(title))
        {
            loadedRecords = loadedRecords.Where(x => x.Title.Contains(title));
        }
        var pagedRecords = await PaginatedList<Blog>.CreateAsync(loadedRecords, pageIndex, pageSize);
        var response = new List<BlogViewDTO>();
        foreach (var blog in pagedRecords)
        {
            var blogImageViewDtos = await ConvertToImageViews(blog.BlogImages);
            var childResponse = new BlogViewDTO(blog, blogImageViewDtos);
            response.Add(childResponse);
        }
        return new PaginatedList<BlogViewDTO>(response, pagedRecords.TotalItems, pageIndex, pageSize);
    }

    public async Task<BaseResponse> UpdateStatusBlog(BlogUpdateStatusDTO dto)
    {
        var repo = _unitOfWork.GetRepo<Blog>();
        var any = await repo.AnyAsync(new QueryBuilder<Blog>()
            .WithPredicate(x => x.Id == dto.Id)
            .Build());
        if (any)
        {
            var blog = await repo.GetSingleAsync(new QueryBuilder<Blog>()
                .WithPredicate(x => x.Id == dto.Id)
                .WithTracking(false)
                .WithInclude(x => x.User)
                .Build());
            blog.Status = (int)dto.Status;
            await repo.UpdateAsync(blog);
            var saver = await _unitOfWork.SaveAsync();
            if (!saver)
            {
                return new BaseResponse { IsSuccess = false, Message = "Lưu dữ liệu không thành công." };
            }
            return new BaseResponse { IsSuccess = true, Message = "Lưu dữ liệu thành công." };
        }
        return new BaseResponse { IsSuccess = false, Message = "Ảnh không tồn tại." };
    }

    public async Task<BaseResponse> AddImagesToBlog(BlogImageRequestDTO dto)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            var repo = _unitOfWork.GetRepo<BlogImage>();
            var createdBlogImageDTOs = new List<BlogImageCreateDTO>();
            foreach (var image in dto.ImagesId)
            {
                var any = await repo.AnyAsync(new QueryBuilder<BlogImage>()
                    .WithPredicate(x => x.ImageId == image && x.BlogId == dto.BlogId)
                    .WithTracking(false)
                    .Build());

                if (any) return new BaseResponse { IsSuccess = false, Message = $"Ảnh {image} đã tổn tại trong Blog." };
                var createdBlogImageDTO = new BlogImageCreateDTO()
                {
                    BlogId = dto.BlogId,
                    ImageId = image
                };
                createdBlogImageDTOs.Add(createdBlogImageDTO);
            }
            var blogImages = _mapper.Map<List<BlogImage>>(createdBlogImageDTOs);
            await repo.CreateAllAsync(blogImages);
            var saver = await _unitOfWork.SaveAsync();
            await _unitOfWork.CommitTransactionAsync();
            if (!saver) return new BaseResponse { IsSuccess = false, Message = "Lưu thất bại" };
            return new BaseResponse { IsSuccess = true, Message = "Lưu thành công." };
        }
        catch (Exception)
        {
            await _unitOfWork.RollBackAsync();
            throw;
        }
    }

    public async Task<BaseResponse> DeleteImagesFromBlog(BlogImageDeleteDTO dto)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            var repo = _unitOfWork.GetRepo<BlogImage>();
            var blogRepo = _unitOfWork.GetRepo<Blog>();
            var deletedBlogImages = new List<BlogImage>();
            var any = await blogRepo.AnyAsync(new QueryBuilder<Blog>()
                .WithPredicate(x => x.Id == dto.BlogId)
                .WithTracking(false)
                .Build());
            if (any)
            {
                foreach (var blogImageId in dto.BlogImageIds)
                {
                    var deleteBlogImage = await repo.GetSingleAsync(new QueryBuilder<BlogImage>()
                        .WithPredicate(x => x.Id == blogImageId && x.BlogId == dto.BlogId)
                        .WithTracking(false)
                        .Build());
                    if (deleteBlogImage == null)
                        return new BaseResponse
                            { IsSuccess = false, Message = $"Ảnh {blogImageId} không tồn tại trong Blog" };
                    deletedBlogImages.Add(deleteBlogImage);
                }

                await repo.DeleteAllAsync(deletedBlogImages);
                var saver = await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();
                if (!saver) return new BaseResponse { IsSuccess = false, Message = "Lưu dữ liệu thất bại" };
                return new BaseResponse { IsSuccess = true, Message = "Lưu dữ liệu thành công." };
            }
            return new BaseResponse { IsSuccess = false, Message = "Blog không tồn tại." };
        }
        catch (Exception)
        {
            await _unitOfWork.RollBackAsync();
            throw;
        }
    }

    public async Task<BlogViewDTO> GetBlogById(int id)
    {
        var repo = _unitOfWork.GetRepo<Blog>();
        var blog = await repo.GetSingleAsync(new QueryBuilder<Blog>()
            .WithPredicate(x => x.Id == id && x.IsActive == true)
            .WithTracking(false)
            .WithInclude(x => x.User, r => r.BlogImages)
            .Build());
        if (blog == null) return null;
        var blogImageViewDtos = await ConvertToImageViews(blog.BlogImages);
        var response = new BlogViewDTO(blog, blogImageViewDtos);
        return response;
    }

    public async Task<BaseResponse> DeleteBlog(int id, string userId)
    {
        var repo = _unitOfWork.GetRepo<Blog>();
        var any = await repo.AnyAsync(new QueryBuilder<Blog>()
            .WithPredicate(x => x.Id == id)
            .Build());
        if (any)
        {
            var blog = await repo.GetSingleAsync(new QueryBuilder<Blog>()
                .WithPredicate(x => x.Id == id)
                .WithTracking(false)
                .WithInclude(x => x.User)
                .Build());
            if (!blog.UserId.Equals(userId)) return new BaseResponse { IsSuccess = false, Message = "Blog không thuộc sở hữu người dùng." };
            blog.IsActive = false;
            await repo.UpdateAsync(blog);
            var saver = await _unitOfWork.SaveAsync();
            if (!saver)
            {
                return new BaseResponse { IsSuccess = false, Message = "Lưu dữ liệu không thành công." };
            }
            return new BaseResponse { IsSuccess = true, Message = "Lưu dữ liệu thành công." };
        }
        return new BaseResponse { IsSuccess = false, Message = "Ảnh không tồn tại." };
        
    }

    
    
    #region Private

    private async Task<List<ImageViewDTO>> ConvertToImageViews(ICollection<BlogImage> blogImages)
    {
        var imageRepo = _unitOfWork.GetRepo<Image>();
        var repsonseImages = new List<ImageViewDTO>();
        foreach (var blogImage in blogImages)
        {
            var image = await imageRepo.GetSingleAsync(new QueryBuilder<Image>()
                .WithPredicate(x => x.Id == blogImage.ImageId)
                .WithInclude(x => x.User)
                .WithTracking(false)
                .Build());
            var childResponseImage = _mapper.Map<ImageViewDTO>(image);
            childResponseImage.UserName = image.User.UserName;
            childResponseImage.CreatedDate = image.CreatedDate.ToString("dd/MM/yyyy");
            repsonseImages.Add(childResponseImage);
        }

        return repsonseImages;
    }

    #endregion
}