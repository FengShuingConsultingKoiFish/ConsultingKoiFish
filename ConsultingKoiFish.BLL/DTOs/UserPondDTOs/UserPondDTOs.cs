namespace ConsultingKoiFish.BLL.DTOs;

public class UserPondDTOs
{
    public string PondName { get; set; } = null!;
    public int Quantity { get; set; }
    public string? Description { get; set; }
    public string? Image { get; set; }
}