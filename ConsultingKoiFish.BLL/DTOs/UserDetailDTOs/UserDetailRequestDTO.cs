using ConsultingKoiFish.BLL.Helpers.Validations.UserDetailValidations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.BLL.DTOs.UserDetailDTOs
{
	public class UserDetailRequestDTO
	{
        [Required(ErrorMessage = "Tên không được để trống.")]
        public string FullName { get; set; }
        [IdentityCardValid]
        public string? IdentityCard { get; set; }
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Avatar { get; set; }
    }
}
