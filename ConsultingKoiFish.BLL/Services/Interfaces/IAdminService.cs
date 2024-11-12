using ConsultingKoiFish.BLL.DTOs.AdminDTOs;

namespace ConsultingKoiFish.BLL.Services.Interfaces;

public interface IAdminService
{
    Task<TotalStasticInYearDTO> GetTotalStatisticInYear();
}