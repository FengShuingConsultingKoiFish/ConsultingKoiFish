using AutoMapper;
using ConsultingKoiFish.BLL.DTOs.BlogDTOs;
using ConsultingKoiFish.BLL.DTOs.BlogImageDTOs;
using ConsultingKoiFish.BLL.DTOs.ImageDTOs;
using ConsultingKoiFish.BLL.DTOs.Response;
using ConsultingKoiFish.BLL.Services.Interfaces;
using ConsultingKoiFish.DAL.Entities;
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
    public async Task<BaseResponse> CraeteUpdateBlog(BlogRequestDTO dto, string userId)
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
                await repo.CreateAsync(createdBlog);
                await _unitOfWork.SaveChangesAsync();

                if (dto.ImageIds != null || dto.ImageIds.Any())
                {
                    foreach (var image in dto.ImageIds)
                    {
                        var createdBlogImageDto = new BlogImageCreateDTO
                        {
                            BlogId = createdBlog.Id,
                            ImageId = image
                        };
                        var createdBlogImage = _mapper.Map<BlogImage>(createdBlogImageDto);
                        await blogImageRepo.CreateAsync(createdBlogImage);
                    }
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

    public async Task<PaginatedList<BlogViewDTO>> GetAllBlogs(int pageIndex, int pageSize)
    {
        var repo = _unitOfWork.GetRepo<Blog>();
        var imageRepo = _unitOfWork.GetRepo<Image>();
        var loadedRecords = repo.Get(new QueryBuilder<Blog>()
            .WithTracking(false)
            .WithInclude(x => x.User, r => r.BlogImages)
            .Build());
        var pagedRecords = await PaginatedList<Blog>.CreateAsync(loadedRecords, pageIndex, pageSize);
        var response = new List<BlogViewDTO>();
        foreach (var blog in pagedRecords)
        {
            var childResponse = _mapper.Map<BlogViewDTO>(blog);
            childResponse.UserName = blog.User.UserName;
            childResponse.CreatedDate = blog.CreatedDate.ToString("dd/MM/yyyy");
            
            var blogImages = blog.BlogImages;
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

            childResponse.ImageViewDtos = repsonseImages;
            response.Add(childResponse);
        }
        return new PaginatedList<BlogViewDTO>(response, pagedRecords.TotalItems, pageIndex, pageSize);
    }
}