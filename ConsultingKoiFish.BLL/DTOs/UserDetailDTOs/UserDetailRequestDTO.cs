using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.BLL.DTOs.UserDetailDTOs
{
	public class UserDetailRequestDTO
	{
        public string? FullName { get; set; }
        public string? IdentityCard { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Avatar { get; set; }
    }
}
