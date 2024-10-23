using AutoMapper;
using ConsultingKoiFish.BLL.DTOs.BlogDTOs;
using ConsultingKoiFish.BLL.DTOs.BlogImageDTOs;
using ConsultingKoiFish.BLL.DTOs.CommentDTOs;
using ConsultingKoiFish.BLL.DTOs.ImageDTOs;
using ConsultingKoiFish.BLL.DTOs.Response;
using ConsultingKoiFish.BLL.Services.Interfaces;
using ConsultingKoiFish.DAL.Entities;
using ConsultingKoiFish.DAL.Enums;
using ConsultingKoiFish.DAL.Paging;
using ConsultingKoiFish.DAL.Queries;
using ConsultingKoiFish.DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace ConsultingKoiFish.BLL.Services.Implements;

public class BlogService : IBlogService
{
	private readonly IUnitOfWork _unitOfWork;
	private readonly IMapper _mapper;
	private readonly IImageService _imageService;
	private readonly ICommentService _commentService;

	public BlogService(IUnitOfWork unitOfWork, IMapper mapper, IImageService imageService, ICommentService commentService)
	{
		_unitOfWork = unitOfWork;
		_mapper = mapper;
		this._imageService = imageService;
		this._commentService = commentService;
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
	public async Task<PaginatedList<BlogViewDTO>> GetAllBlogs(BlogGetListDTO dto)
	{
		var repo = _unitOfWork.GetRepo<Blog>();
		var imageRepo = _unitOfWork.GetRepo<Image>();
		var loadedRecords = repo.Get(new QueryBuilder<Blog>()
			.WithPredicate(x => x.IsActive == true && x.Status == (int)BlogStatus.Approved)
			.WithTracking(false)
			.WithInclude(x => x.User, y => y.BlogImages)
			.Build());
		if (dto.Title != null)
		{
			loadedRecords = loadedRecords.Where(x => x.Title.Contains(dto.Title));
		}

		if (dto.OrderBlog.HasValue)
		{
			switch ((int)dto.OrderBlog)
			{
				case 1: loadedRecords = loadedRecords.OrderByDescending(x => x.CreatedDate); break;
				case 2: loadedRecords = loadedRecords.OrderBy(x => x.CreatedDate); break;
			}
		}
		var pagedRecords = await PaginatedList<Blog>.CreateAsync(loadedRecords, dto.PageIndex, dto.PageSize);
		var response = await ConvertBlogsToBlogViews(pagedRecords, dto.OrderComment, dto.OrderImage);
		return new PaginatedList<BlogViewDTO>(response, pagedRecords.TotalItems, dto.PageIndex, dto.PageSize);
	}

	//thằng này get hết blog của th user
	public async Task<PaginatedList<BlogViewDTO>> GetAllBlogsByUserId(string userId, BlogGetListDTO dto)
	{
		var repo = _unitOfWork.GetRepo<Blog>();
		var imageRepo = _unitOfWork.GetRepo<Image>();
		var loadedRecords = repo.Get(new QueryBuilder<Blog>()
			.WithPredicate(x => x.IsActive == true && x.UserId.Equals(userId))
			.WithTracking(false)
			.WithInclude(x => x.User, r => r.BlogImages)
			.Build());

		if (dto.Title != null)
		{
			loadedRecords = loadedRecords.Where(x => x.Title.Contains(dto.Title));
		}

		if (dto.BlogStatus.HasValue)
		{
			loadedRecords = loadedRecords.Where(x => x.Status == (int)dto.BlogStatus);
		}

		if (dto.OrderBlog.HasValue)
		{
			switch ((int)dto.OrderBlog)
			{
				case 1: loadedRecords = loadedRecords.OrderByDescending(x => x.CreatedDate); break;
				case 2: loadedRecords = loadedRecords.OrderBy(x => x.CreatedDate); break;
			}
		}
		var pagedRecords = await PaginatedList<Blog>.CreateAsync(loadedRecords, dto.PageIndex, dto.PageSize);
		var response = await ConvertBlogsToBlogViews(pagedRecords, dto.OrderComment, dto.OrderImage);
		return new PaginatedList<BlogViewDTO>(response, pagedRecords.TotalItems, dto.PageIndex, dto.PageSize);
	}

	public async Task<PaginatedList<BlogViewDTO>> GetAllBlogsForAdmin(BlogGetListDTO dto)
	{
		var repo = _unitOfWork.GetRepo<Blog>();
		var imageRepo = _unitOfWork.GetRepo<Image>();
		var loadedRecords = repo.Get(new QueryBuilder<Blog>()
			.WithPredicate(x => x.IsActive == true)
			.WithTracking(false)
			.WithInclude(x => x.User, r => r.BlogImages)
			.Build());

		if (dto.Title != null)
		{
			loadedRecords = loadedRecords.Where(x => x.Title.Contains(dto.Title));
		}

		if (dto.BlogStatus.HasValue)
		{
			loadedRecords = loadedRecords.Where(x => x.Status == (int)dto.BlogStatus);
		}

		if (dto.OrderBlog.HasValue)
		{
			switch ((int)dto.OrderBlog)
			{
				case 1: loadedRecords = loadedRecords.OrderByDescending(x => x.CreatedDate); break;
				case 2: loadedRecords = loadedRecords.OrderBy(x => x.CreatedDate); break;
			}
		}
		var pagedRecords = await PaginatedList<Blog>.CreateAsync(loadedRecords, dto.PageIndex, dto.PageSize);
		var response = await ConvertBlogsToBlogViews(pagedRecords, dto.OrderComment, dto.OrderImage);
		return new PaginatedList<BlogViewDTO>(response, pagedRecords.TotalItems, dto.PageIndex, dto.PageSize);
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
		return new BaseResponse { IsSuccess = false, Message = "Blog không tồn tại." };
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
				foreach (var imageId in dto.ImageIds)
				{
					var deleteBlogImage = await repo.GetSingleAsync(new QueryBuilder<BlogImage>()
						.WithPredicate(x => x.Id == imageId && x.BlogId == dto.BlogId)
						.WithTracking(false)
						.Build());
					if (deleteBlogImage == null)
						return new BaseResponse
						{ IsSuccess = false, Message = $"Ảnh {imageId} không tồn tại trong Blog" };
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

	public async Task<BlogViewDTO> GetBlogById(int id, OrderComment? orderComment, OrderImage? orderImage)
	{
		var repo = _unitOfWork.GetRepo<Blog>();
		var blog = await repo.GetSingleAsync(new QueryBuilder<Blog>()
			.WithPredicate(x => x.Id == id && x.IsActive == true)
			.WithTracking(false)
			.WithInclude(x => x.User, r => r.BlogImages, z => z.BlogComments)
			.Build());
		if (blog == null) return null;
		var response = await ConvertBlogToBlogView(blog, orderComment, orderImage);
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
		return new BaseResponse { IsSuccess = false, Message = "Blog không tồn tại." };

	}

	#region PRIVATE

	/// <summary>
	/// This is used to get blog images for each blog
	/// </summary>
	/// <param name="blog"></param>
	/// <param name="orderImage"></param>
	/// <returns></returns>
	private async Task<List<BlogImage>> GetBlogImagesForEachBlog(Blog blog, OrderImage? orderImage)
	{
		var blogImageRepo = _unitOfWork.GetRepo<BlogImage>();
		var blogImages = blogImageRepo.Get(new QueryBuilder<BlogImage>()
												.WithPredicate(x => x.BlogId == blog.Id)
												.WithTracking(false)
												.Build());
		if (orderImage.HasValue)
		{
			switch ((int)orderImage)
			{
				case 1: blogImages = blogImages.OrderByDescending(x => x.Id); break;
				case 2: blogImages = blogImages.OrderBy(x => x.Id); break;
			}
		}
		return await blogImages.ToListAsync();
	}


	/// <summary>
	/// This is used to convert blog images to imageViewDTOs
	/// </summary>
	/// <param name="blogImages"></param>
	/// <returns></returns>
	private Task<List<ImageViewDTO>> ConvertBlogImagesToImageViews(ICollection<BlogImage> blogImages)
	{
		return _imageService.ConvertSpeciedImageToImageViews(blogImages, blogImage => blogImage.ImageId);
	}


	/// <summary>
	/// This is used to get an collection of BlogComment for each Blog
	/// </summary>
	/// <param name="blog"></param>
	/// <returns></returns>
	private async Task<List<BlogComment>> GetBlogCommentsForEachBlog(Blog blog, OrderComment? orderComment)
	{
		var blogCommentRepo = _unitOfWork.GetRepo<BlogComment>();
		var blogComments = blogCommentRepo.Get(new QueryBuilder<BlogComment>()
												.WithPredicate(x => x.BlogId == blog.Id)
												.WithTracking(false)
												.Build());
		if (orderComment.HasValue)
		{
			switch ((int)orderComment)
			{
				case 1: blogComments = blogComments.OrderByDescending(x => x.Id); break;
				case 2: blogComments = blogComments.OrderBy(x => x.Id); break;
			}
		}
		return await blogComments.ToListAsync();
	}

	/// <summary>
	/// this is used to convert blog comments to commentViewDTOs
	/// </summary>
	/// <param name="blogComments"></param>
	/// <returns></returns>
	private Task<List<CommentViewDTO>> ConvertBlogCommentsToCommentViews(ICollection<BlogComment> blogComments)
	{
		var commentViews = _commentService.ConvertSpeciedCommentToCommentViews(blogComments, blogComment => blogComment.CommentId);
		return commentViews;
	}

	/// <summary>
	/// This is used to convert a collection of blogs to a collection of blogViewDtos
	/// </summary>
	/// <param name="blogs"></param>
	/// <returns></returns>
	private async Task<List<BlogViewDTO>> ConvertBlogsToBlogViews(List<Blog> blogs, OrderComment? orderComment, OrderImage? orderImage)
	{
		var response = new List<BlogViewDTO>();
		foreach (var blog in blogs)
		{
			var blogImageViewDtos = await ConvertBlogImagesToImageViews(await GetBlogImagesForEachBlog(blog, orderImage));
			var blogCommentViewDtos = await ConvertBlogCommentsToCommentViews(await GetBlogCommentsForEachBlog(blog, orderComment));
			var childResponse = new BlogViewDTO(blog, blogImageViewDtos, blogCommentViewDtos);
			response.Add(childResponse);
		}
		return response;
	}

	/// <summary>
	/// This is used to convert a blog to blogViewDto
	/// </summary>
	/// <param name="blog"></param>
	/// <returns></returns>
	private async Task<BlogViewDTO> ConvertBlogToBlogView(Blog blog, OrderComment? orderComment, OrderImage? orderImage)
	{
		var blogImageViewDtos = await ConvertBlogImagesToImageViews(await GetBlogImagesForEachBlog(blog, orderImage));
		var blogCommentViewDtos = await ConvertBlogCommentsToCommentViews(await GetBlogCommentsForEachBlog(blog, orderComment));
		var response = new BlogViewDTO(blog, blogImageViewDtos, blogCommentViewDtos);
		return response;
	}
	#endregion
}