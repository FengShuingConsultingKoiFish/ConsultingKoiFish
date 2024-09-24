using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.BLL.DTOs.Response
{
	public class ResponseApiDTO
	{
		public HttpStatusCode StatusCode { get; set; }
		public bool IsSuccess { get; set; }
		public string? Message { get; set; }
		public List<KeyValuePair<string, string>>? Errors { get; set; }
		public object? Result { get; set; }
	}
}
