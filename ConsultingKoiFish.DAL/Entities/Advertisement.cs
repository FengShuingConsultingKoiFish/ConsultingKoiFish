﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.DAL.Entities
{
	public partial class Advertisement
	{
		public int Id { get; set; }
		public string UserId { get; set; }
		public string Title { get; set; } = null!;
		[AllowNull]
		public string? Description { get; set; }
		[AllowNull]
		public double Price { get; set; }
		public DateTime CreatedDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;

        public virtual ApplicationUser User { get; set; }
		public virtual ICollection<AdAttribute> AdAttributes { get; set; } = new List<AdAttribute>();
		public virtual ICollection<AdImage> AdImages { get; set; } = new List<AdImage>();
		public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
	}
}
