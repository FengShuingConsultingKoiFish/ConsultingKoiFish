using ConsultingKoiFish.BLL.DTOs.PaymentDTOs;
using ConsultingKoiFish.BLL.DTOs.Response;
using ConsultingKoiFish.DAL.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.BLL.Services.Interfaces
{
	public interface IPaymentService
	{
		Task<BaseResponse> CreatePayment(PaymentCreateDTO dto);
		Task<PaginatedList<PaymentViewDTO>> GetAllPaymentsByUserId(PaymentGetListDTO dto, string userId);
		Task<PaginatedList<PaymentViewDTO>> GetAllPaymentsByUserIdWithDate(PaymentGetListDTO dto, string userId);
		Task<PaginatedList<PaymentViewDTO>> GetAllPayments(PaymentGetListDTO dto);
		Task<PaginatedList<PaymentViewDTO>> GetAllPaymentsWithDate(PaymentGetListDTO dto);
		Task<PaymentViewDTO> GetPaymentByIdForMember(int id, string userId);
		Task<PaymentViewDTO> GetPaymentByIdForAdmin(int id);
	}
}
