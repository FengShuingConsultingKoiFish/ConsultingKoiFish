using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.DAL.Entities
{
	public partial class AdAttribute
	{
		public int Id { get; set; }
		[AllowNull]
		public int AdvertisementId { get; set; }
		[AllowNull]
		public string? AttributeName { get; set; }
		[AllowNull]
		public string? AttributeValue { get; set; }

		public virtual Advertisement Advertisement { get; set; }
	}
}
