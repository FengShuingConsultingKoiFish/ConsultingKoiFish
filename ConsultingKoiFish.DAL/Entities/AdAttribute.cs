using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.DAL.Entities
{
	public partial class AdAttribute
	{
		public int Id { get; set; }
		public int AdvertisementId { get; set; }
		public string? AttributeName { get; set; }
		public string? AttributeValue { get; set; }

		public virtual Advertisement Advertisement { get; set; }
	}
}
