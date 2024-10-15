using ConsultingKoiFish.BLL.Helpers.Validations.AccountValidations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ConsultingKoiFish.BLL.DTOs.AccountDTOs;

public class AccountCreateRequestDTO
{
    [Required(ErrorMessage = "Email không được bỏ trống")]
    [EmailAddress(ErrorMessage = "Email sai định dạng. Định dạng đúng: example@gmail.com")]
    public string EmailAddress { get; set; } = null!;

    [Required(ErrorMessage = "UserNameOrEmail không được để trống")]
    public string UserName { get; set; } = null!;

    [PasswordValidation]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "Nhắc lại mật khẩu không được để trống.")]
    public string ConfirmPassword { get; set; } = null!;

    [Required(ErrorMessage = "Số điện thoại không được để trống.")]
    [Phone(ErrorMessage = "Số điện thoại không đúng định dạng")]
    public string PhoneNumber { get; set; } = null!;
}