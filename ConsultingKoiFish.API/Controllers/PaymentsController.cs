using ConsultingKoiFish.BLL.DTOs.AdvertisementPackageDTOs;
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



		#region Member

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
		[Route("get-all-payments-by-userId")]
		public async Task<IActionResult> GetAllPaymentsByUserId([FromBody] PaymentGetListDTO dto)
		{
			try
			{
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

				var data = await _paymentService.GetAllPaymentsByUserId(dto, UserId);
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

		[Authorize(Roles = "Member")]
		[HttpPost]
		[Route("filter-all-payments-by-userId-with-date")]
		public async Task<IActionResult> GetAllPaymentsByUserIdWithDate([FromBody] PaymentGetListDTO dto)
		{
			try
			{
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

				var data = await _paymentService.GetAllPaymentsByUserIdWithDate(dto, UserId);
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
		#endregion


		[HttpPost]
		[Route("request-payment")]
		public IActionResult RequestPayment(VnPayRequestDTO requestDTO)
		{
			var paymentUrl = _vnPayService.CreatePaymentUrl(HttpContext, requestDTO);
			if (paymentUrl == null) return GetError("Nhận url thanh toán thất bại. Vui lòng thử lại sau ít phút.");
			return Ok(new {Url = paymentUrl});
		}

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
