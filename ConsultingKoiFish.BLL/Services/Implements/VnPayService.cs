using ConsultingKoiFish.BLL.DTOs.VnPayDTOs;
using ConsultingKoiFish.BLL.Helpers.Config;
using ConsultingKoiFish.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.BLL.Services.Implements
{
	public class VnPayService : IVnPayService
	{
		private readonly VnPayConfiguration _vnPayConfig;

		public VnPayService(VnPayConfiguration vnPayConfig)
		{
			_vnPayConfig = vnPayConfig;
		}

		public string CreatePaymentUrl(HttpContext context, VnPayRequestDTO vnPayRequest)
		{
			vnPayRequest.OrderId = DateTime.Now.Ticks;
			var vnpay = new VnPayLibrary();

			vnpay.AddRequestData("vnp_Version", _vnPayConfig.Version);
			vnpay.AddRequestData("vnp_Command", _vnPayConfig.Command);
			vnpay.AddRequestData("vnp_TmnCode", _vnPayConfig.TmnCode);
			vnpay.AddRequestData("vnp_Amount", (vnPayRequest.Amount * 100).ToString()); //Số tiền thanh toán. Số tiền không 
																						//mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND
																						//(một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần(khử phần thập phân), sau đó gửi sang VNPAY
																						//là: 10000000

			Console.BackgroundColor = ConsoleColor.Green;
			Console.WriteLine(vnPayRequest.Amount);
			Console.ResetColor();

			vnpay.AddRequestData("vnp_CreateDate", vnPayRequest.CreatedDate.ToString("yyyyMMddHHmmss"));
			vnpay.AddRequestData("vnp_CurrCode", _vnPayConfig.CurrCode);
			vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(context));
			vnpay.AddRequestData("vnp_Locale", _vnPayConfig.Locale);

			vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán cho đơn hàng: " + vnPayRequest.OrderId);
			vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other
			vnpay.AddRequestData("vnp_ReturnUrl", _vnPayConfig.ReturnUrl);
			vnpay.AddRequestData("vnp_TxnRef", vnPayRequest.OrderId.ToString());

			var paymentUrl = vnpay.CreateRequestUrl(_vnPayConfig.PaymentUrl, _vnPayConfig.HashSecret);

			return paymentUrl;
		}

		public VnPayResponseDTO PaymentExcute(IQueryCollection collection)
		{
			var vnpay = new VnPayLibrary();


			foreach (var (key, value) in collection)
			{
				if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
				{
					vnpay.AddResponseData(key, value.ToString());
				}
			}

			var vnp_orderId = Convert.ToInt64(vnpay.GetResponseData("vnp_TxnRef"));
			var vnp_TransactionId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
			var vnp_SecureHash = collection.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value;
			var vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
			var vnp_OrderInfo = vnpay.GetResponseData("vnp_OrderInfo");
			var vnp_Amount = vnpay.GetResponseData("vnp_Amount");

			bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, _vnPayConfig.HashSecret);

			if (!checkSignature)
			{
				return new VnPayResponseDTO
				{
					IsSuccess = false
				};
			}

			var result = new VnPayResponseDTO
			{

				IsSuccess = true,
				PaymentMethod = "VnPay",
				OrderDescription = vnp_OrderInfo,
				OrderId = vnp_orderId.ToString(),
				TransactionId = vnp_TransactionId,
				Token = vnp_SecureHash,
				VnPayResponseCode = vnp_ResponseCode,
				Amount = Double.Parse(vnp_Amount),
			};

			return result;
		}
	}
}
