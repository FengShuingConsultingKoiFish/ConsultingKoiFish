using ConsultingKoiFish.BLL.Helpers.Validations.UserDetailValidations;
using ConsultingKoiFish.DAL.Entities;
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
		public string UserId { get; set; }
		public string? UserName { get; set; }
		public string? FullName { get; set; }
		public string? IdentityCard { get; set; }
		public string? DateOfBirth { get; set; }
		public string? Gender { get; set; }
		public string? Avatar { get; set; }
		public string? CreatedDate { get; set; }

		public UserDetailViewDTO(UserDetail userDetail)
		{
			UserId = userDetail.UserId;
			UserName = userDetail.User.UserName;
			FullName = userDetail?.FullName;
			IdentityCard = userDetail?.IdentityCard;
			DateOfBirth = userDetail.DateOfBirth.HasValue ? userDetail.DateOfBirth.Value.ToString("dd/MM/yyyy") : null;
			Gender = userDetail?.Gender;
			Avatar = userDetail?.Avatar;
			CreatedDate = userDetail.CreatedDate.HasValue ? userDetail.CreatedDate.Value.ToString("dd/MM/yyyy") : null;
		}
	}
}
