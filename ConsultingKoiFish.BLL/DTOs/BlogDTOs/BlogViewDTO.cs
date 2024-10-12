using ConsultingKoiFish.BLL.DTOs.CommentDTOs;
using ConsultingKoiFish.BLL.DTOs.ImageDTOs;
using ConsultingKoiFish.DAL.Entities;
using ConsultingKoiFish.DAL.Enums;

namespace ConsultingKoiFish.BLL.DTOs.BlogDTOs;

public class BlogViewDTO
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string? CreatedDate { get; set; }
    public string? Status { get; set; }
    public List<ImageViewDTO> ImageViewDtos { get; set; } = new List<ImageViewDTO>();
    public List<CommentViewDTO> CommentViewDtos { get; set; } = new List<CommentViewDTO>();

    public BlogViewDTO(Blog blog, List<ImageViewDTO> blogImageViewDtos, List<CommentViewDTO> blogCommentViewDtos)
    {
        Id = blog.Id;
        Title = blog.Title;
        Content = blog.Content;
        UserName = blog.User.UserName;
        CreatedDate = blog.CreatedDate.ToString("dd/MM/yyyy");
        var statusResponse = (BlogStatus)blog.Status;
        Status = statusResponse.ToString();
        ImageViewDtos = blogImageViewDtos;
        CommentViewDtos = blogCommentViewDtos;
    }

}