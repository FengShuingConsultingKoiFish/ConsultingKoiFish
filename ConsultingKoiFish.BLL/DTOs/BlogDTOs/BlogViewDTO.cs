using ConsultingKoiFish.BLL.DTOs.ImageDTOs;

namespace ConsultingKoiFish.BLL.DTOs.BlogDTOs;

public class BlogViewDTO
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string? CreatedDate { get; set; }
    public List<ImageViewDTO> ImageViewDtos { get; set; } = new List<ImageViewDTO>();

}