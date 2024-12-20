﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.DAL.Entities
{
	public partial class BlogComment
	{
		public int Id { get; set; }
        public int CommentId { get; set; }
        public int BlogId { get; set; }

		public virtual Blog Blog { get; set; }
		public virtual Comment Comment { get; set; }

	}
}
