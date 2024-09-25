using ConsultingKoiFish.BLL.DTOs.AccountDTOs;
using ConsultingKoiFish.BLL.DTOs.Response;

namespace ConsultingKoiFish.BLL.Services.Implements;

public interface IAccountService
{
    public Task<AccountViewDTO> SignUpAsync(AccountCreateRequestDTO accRequest);
}