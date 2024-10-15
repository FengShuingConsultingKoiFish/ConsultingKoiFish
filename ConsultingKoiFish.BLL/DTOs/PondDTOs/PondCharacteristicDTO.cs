namespace ConsultingKoiFish.BLL.DTOs.PondDTOs;

public class PondCharacteristicDTO
{
    public int PondCategoryId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? Image { get; set; }
}