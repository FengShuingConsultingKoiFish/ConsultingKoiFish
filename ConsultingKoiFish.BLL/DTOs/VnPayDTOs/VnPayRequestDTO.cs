﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.BLL.DTOs.VnPayDTOs
{
	public class VnPayRequestDTO
	{
		public long OrderId { get; set; }
		public string FullName { get; set; }
		public string Description { get; set; }
		public double Amount { get; set; }
		public DateTime CreatedDate { get; set; }
	}
}
