﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.DAL.Entities
{
	public partial class Blog
	{
		public int Id { get; set; }
		public string Title { get; set; } = null!;
		public string Content { get; set; } = null!;
		public string UserId { get; set; }
		public DateTime CreatedDate { get; set; }
		public int Status { get; set; }
		public bool IsActive { get; set; } = true;

		public virtual ApplicationUser User { get; set; }
		public virtual ICollection<BlogImage> BlogImages { get; set; } = new List<BlogImage>();
		public virtual ICollection<BlogComment> BlogComments { get; set; } = new List<BlogComment>();
	}
}
