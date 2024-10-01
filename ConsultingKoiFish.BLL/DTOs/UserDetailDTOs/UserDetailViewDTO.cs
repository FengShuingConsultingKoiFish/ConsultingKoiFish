using ConsultingKoiFish.BLL.Helpers.Validations.UserDetailValidations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.BLL.DTOs.UserDetailDTOs
{
	public class UserDetailViewDTO
	{
		public string? FullName { get; set; }
		public string? IdentityCard { get; set; }
		public DateTime? DateOfBirth { get; set; }
		public string? Gender { get; set; }
		public string? Avatar { get; set; }
		public DateTime? CreatedDate { get; set; }
	}
}
