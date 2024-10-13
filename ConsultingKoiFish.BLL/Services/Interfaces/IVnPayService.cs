using ConsultingKoiFish.BLL.DTOs.VnPayDTOs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.BLL.Services.Interfaces
{
	public interface IVnPayService
	{
		string CreatePaymentUrl(HttpContext context, VnPayRequestDTO vnPayRequest);
		VnPayResponseDTO PaymentExcute(IQueryCollection collection);
	}
}
