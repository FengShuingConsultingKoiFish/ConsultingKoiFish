﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.DAL.Entities
{
	public partial class Payment
	{
		public int Id { get; set; }
		public string UserId { get; set; }
		public int AdvertisementPackageId { get; set; }
		public long TransactionId { get; set; }
		[AllowNull]
		public string? Content { get; set; }
		public double Amount { get; set; }
		[Required]
		public string ClonePackage { get; set; } = null!;
		public DateTime CreatedDate { get; set; }

		public virtual ApplicationUser User { get; set; }
		public virtual AdvertisementPackage AdvertisementPackage { get; set; }
	}
}
