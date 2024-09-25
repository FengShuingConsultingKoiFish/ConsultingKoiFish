namespace ConsultingKoiFish.BLL.DTOs.AccountDTOs;

public class AccountCreateRequestDTO
{
    public string EmailAddress { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string ConfirmPassword { get; set; } = null!;
}