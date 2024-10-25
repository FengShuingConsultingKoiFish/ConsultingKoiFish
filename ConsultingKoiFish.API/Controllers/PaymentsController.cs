﻿using ConsultingKoiFish.BLL.DTOs.AdvertisementPackageDTOs;
using ConsultingKoiFish.BLL.DTOs;
using ConsultingKoiFish.BLL.DTOs.PaymentDTOs;
using ConsultingKoiFish.BLL.DTOs.PurchasedPackageDTOs;
using ConsultingKoiFish.BLL.DTOs.VnPayDTOs;
using ConsultingKoiFish.BLL.Helpers;
using ConsultingKoiFish.BLL.Services.Implements;
using ConsultingKoiFish.BLL.Services.Interfaces;
using ConsultingKoiFish.DAL.Entities;
using ConsultingKoiFish.DAL.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Printing;
using System.Net.Http;
using ConsultingKoiFish.BLL.DTOs.BlogDTOs;

namespace ConsultingKoiFish.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PaymentsController : BaseAPIController
	{
		private readonly IVnPayService _vnPayService;
		private readonly IPaymentService _paymentService;
		private readonly IPurchasedPackageService _purchasedPackageService;
		private readonly IUserDetailService _userDetailService;
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IConfiguration _configuration;

		public PaymentsController(IVnPayService vnPayService,
			IPaymentService paymentService, IPurchasedPackageService purchasedPackageService,
			IUserDetailService userDetailService, IHttpClientFactory httpClientFactory,
			IConfiguration configuration)
		{
			this._vnPayService = vnPayService;
			this._paymentService = paymentService;
			this._purchasedPackageService = purchasedPackageService;
			this._userDetailService = userDetailService;
			this._httpClientFactory = httpClientFactory;
			this._configuration = configuration;
		}

		#region Admin

		/// <summary>
		/// Get all payments for admin
		/// </summary>
		/// <param name="dto"></param>
		/// <returns></returns>
		[Authorize(Roles = "Admin")]
		[HttpPost]
		[Route("get-all-payments-for-admin")]
		public async Task<IActionResult> GetAllPayments([FromBody] PaymentGetListDTO dto)
		{
			try
			{
				if(!ModelState.IsValid) return ModelInvalid();

				if (dto.PageIndex <= 0)
				{
					ModelState.AddModelError("PageIndex", "PageIndex phải là số nguyên dương.");
					return ModelInvalid();
				}

				if (dto.PageSize <= 0)
				{
					ModelState.AddModelError("PageSize", "PageSize phải là số nguyên dương.");
					return ModelInvalid();
				}

				if(!dto.IsValidOrderImage())
				{
					ModelState.AddModelError("OrderImage", "OrderImage không hợp lệ.");
					return ModelInvalid();
				}

				if (!dto.IsValidOrderDate())
				{
					ModelState.AddModelError("OrderDate", "OrderDate không hợp lệ.");
					return ModelInvalid();
				}

				var data = await _paymentService.GetAllPaymentsForAdmin(dto);
				var response = new PagingDTO<PaymentViewDTO>(data);
				if (response == null) return GetError();
				return GetSuccess(response);
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ResetColor();
				return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút nữa.");
			}
		}

		/// <summary>
		/// Get payment detail for admin
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[Authorize(Roles = "Admin")]
		[HttpGet]
		[Route("get-payment-by-id-for-admin/{id}")]
		public async Task<IActionResult> GetPaymentByIdForAdmin([FromRoute] int id, OrderImage? orderImage)
		{
			try
			{
				var response = await _paymentService.GetPaymentByIdForAdmin(id, orderImage);
				if (response == null) return GetError("Payment này không tồn tại.");
				return GetSuccess(response);
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ResetColor();
				return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút nữa.");
			}
		}

		#endregion

		#region Member

		/// <summary>
		/// This is used to purchase a package and make a payment
		/// </summary>
		/// <param name="packageViewDTO"></param>
		/// <returns></returns>
		[Authorize(Roles = "Member")]
		[HttpPost]
		[Route("purchase-package")]
		public async Task<IActionResult> PurchasePackage(AdvertismentPackagePurchaseDTO packageViewDTO)
		{
			var userDetail = await _userDetailService.GetUserDetailByUserId(UserId);
			if (userDetail == null) return GetError("Vui lòng điền đầy đủ thông tin profile để tiếp tục thanh toán.");
			var vnpRequest = new VnPayRequestDTO
			{
				UserId = UserId,
				PackageName = packageViewDTO.Name,
				FullName = userDetail.FullName,
				Description = $"{UserId}/{userDetail.FullName} thanh toan goi {packageViewDTO.Name}. Ma goi {packageViewDTO.PackageId}",
				Amount = packageViewDTO.Price,
				CreatedDate = DateTime.Now,
			};

			var httpClient = _httpClientFactory.CreateClient();
			var responseUrl = await httpClient.PostAsJsonAsync(_configuration["VnPayConfiguration:RequestPaymentUrl"], vnpRequest);
			if (!responseUrl.IsSuccessStatusCode)
			{
				var errorContent = await responseUrl.Content.ReadAsStringAsync();
				return GetError($"Error: {errorContent}");
			}
			var paymentUrl = await responseUrl.Content.ReadFromJsonAsync<PaymentUrlResponseDTO>();
			return GetSuccess(paymentUrl.Url);
		}

		/// <summary>
		/// Get all payment follow userId
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>

		[Authorize(Roles = "Member")]
		[HttpPost]
		[Route("get-all-payments-for-member")]
		public async Task<IActionResult> GetAllPaymentsForMember([FromBody] PaymentGetListDTO dto)
		{
			try
			{
				if (!ModelState.IsValid) return ModelInvalid();

				if (dto.PageIndex <= 0)
				{
					ModelState.AddModelError("PageIndex", "PageIndex phải là số nguyên dương.");
					return ModelInvalid();
				}

				if (dto.PageSize <= 0)
				{
					ModelState.AddModelError("PageSize", "PageSize phải là số nguyên dương.");
					return ModelInvalid();
				}

				if (!dto.IsValidOrderImage())
				{
					ModelState.AddModelError("OrderImage", "OrderImage không hợp lệ.");
					return ModelInvalid();
				}

				if (!dto.IsValidOrderDate())
				{
					ModelState.AddModelError("OrderDate", "OrderDate không hợp lệ.");
					return ModelInvalid();
				}

				var data = await _paymentService.GetAllPaymentsForMember(dto, UserId);
				var response = new PagingDTO<PaymentViewDTO>(data);
				if (response == null) return GetError();
				return GetSuccess(response);
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ResetColor();
				return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút nữa.");
			}
		}


		/// <summary>
		/// Get payment detail for member
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[Authorize(Roles = "Member")]
		[HttpGet]
		[Route("get-payment-by-id-for-member/{id}")]
		public async Task<IActionResult> GetPaymentByIdForMember([FromRoute] int id, OrderImage? orderImage)
		{
			try
			{
				var response = await _paymentService.GetPaymentByIdForMember(id, UserId, orderImage);
				if (response == null) return GetError("Payment này không tồn tại.");
				return GetSuccess(response);
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ResetColor();
				return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút nữa.");
			}
		}

		#endregion

		/// <summary>
		/// This is used to request a payment link
		/// </summary>
		/// <param name="requestDTO"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("request-payment")]
		public IActionResult RequestPayment(VnPayRequestDTO requestDTO)
		{
			var paymentUrl = _vnPayService.CreatePaymentUrl(HttpContext, requestDTO);
			if (paymentUrl == null) return GetError("Nhận url thanh toán thất bại. Vui lòng thử lại sau ít phút.");
			return Ok(new {Url = paymentUrl});
		}

		/// <summary>
		/// This is used to response after payment processing
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("response-payment")]
		public async Task<IActionResult> ResponsePayment()
		{
			try
			{
				var vnPayResponse = _vnPayService.PaymentExcute(Request.Query);
				if (!vnPayResponse.IsSuccess || !vnPayResponse.VnPayResponseCode.Equals("00"))
					return GetError("Lỗi thanh toán VnPay. Vui lòng thử lại sau ít phút.");

				string description = vnPayResponse.OrderDescription;
				string[] parts = description.Split("Ma goi ");
				string packageId = parts[1];

				string[] parts1 = description.Split('/');
				string userId = parts1[0];

				var createdPaymentDTO = new PaymentCreateDTO
				{
					UserId = userId,
					AdvertisementPackageId = Convert.ToInt32(packageId),
					TransactionId = vnPayResponse.TransactionId,
					Content = vnPayResponse.OrderDescription,
					Amount = vnPayResponse.Amount,
					CreatedDate = DateTime.Now,
				};

				var createdPayment = await _paymentService.CreatePayment(createdPaymentDTO);
				if (!createdPayment.IsSuccess) return SaveError(createdPayment);

				var createdPurchasedPackageDTO = new PurchasedPackageCreateDTO
				{
					UserId = userId,
					AdvertisementPackageId = Convert.ToInt32(packageId),
					MornitoredQuantity = 0,
					Status = (int)PurchasedPackageStatus.Available,
					CreatedDate = DateTime.Now,
				};
				var createdPurchasedPackage = await _purchasedPackageService.CreatePurchasedPacakge(createdPurchasedPackageDTO);
				if (!createdPurchasedPackage.IsSuccess) return SaveError(createdPurchasedPackage);
				return GetSuccess(Constants.vnp00);
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ResetColor();
				return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút nữa.");
			}
		}
	}
}
