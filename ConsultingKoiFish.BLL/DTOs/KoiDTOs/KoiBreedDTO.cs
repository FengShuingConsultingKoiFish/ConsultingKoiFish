namespace ConsultingKoiFish.BLL.DTOs.KoiDTOs;

public class KoiBreedDTO
{
    public int KoiCategoryId { get; set; }
    public string Name { get; set; } = null!;
    public string Colors { get; set; }
    public string Pattern { get; set; }
    public string Description { get; set; }
    public string Image { get; set; }
}