﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.DAL.Entities
{
	public partial class Comment
	{
		public int Id { get; set; }
		public string UserId { get; set; }
		public int BlogId { get; set; }
		public int AdvertisementId { get; set; }
		public string Content { get; set; } = null!;
		public DateTime CreatedDate { get; set; }

		public virtual ApplicationUser User { get; set; }
		public virtual Blog Blog { get; set; }
		public virtual Advertisement Advertisement { get; set; }
	}
}