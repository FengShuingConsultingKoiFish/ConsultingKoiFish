namespace ConsultingKoiFish.BLL.DTOs.AdvertisementPackageDTOs;

public class AdvertisementPackageUpdateDTO
{
    public string Name { get; set; } = null!;
    public double Price { get; set; }
    public string? Description { get; set; }
    public int? LimitAd { get; set; }
    public int? LimitContent { get; set; }
    public int? LimitImage { get; set; }
    public int? DurationsInDays { get; set; }
}