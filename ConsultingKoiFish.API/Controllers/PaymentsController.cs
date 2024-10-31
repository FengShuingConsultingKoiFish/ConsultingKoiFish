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
using Microsoft.AspNetCore.Mvc.Infrastructure;
using ConsultingKoiFish.BLL.DTOs.EmailDTOs;
using System.Text.RegularExpressions;

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
		private readonly IAdvertisementPackageService _advertisementPackageService;
		private readonly IEmailService _emailService;
		private readonly IIdentityService _identityService;

		public PaymentsController(IVnPayService vnPayService,
			IPaymentService paymentService, IPurchasedPackageService purchasedPackageService,
			IUserDetailService userDetailService, IHttpClientFactory httpClientFactory,
			IConfiguration configuration, IAdvertisementPackageService advertisementPackageService, IEmailService emailService,
			IIdentityService identityService)
		{
			this._vnPayService = vnPayService;
			this._paymentService = paymentService;
			this._purchasedPackageService = purchasedPackageService;
			this._userDetailService = userDetailService;
			this._httpClientFactory = httpClientFactory;
			this._configuration = configuration;
			this._advertisementPackageService = advertisementPackageService;
			this._emailService = emailService;
			this._identityService = identityService;
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
			var package = await _advertisementPackageService.GetPackageById(packageViewDTO.PackageId, OrderImage.DatetimeDescending);
			if (package == null)
			{
				ModelState.AddModelError("PackageId", "Gói bạn chọn không còn tồn tại.");
				return ModelInvalid();
			}
			var vnpRequest = new VnPayRequestDTO
			{
				UserId = UserId,
				PackageName = package.Name,
				FullName = userDetail.FullName,
				Description = $"{UserId}/{userDetail.FullName} thanh toan goi {package.Name}. Ma goi {package.Id}",
				Amount = package.Price,
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
		/// This is used to extend the unavailable package
		/// </summary>
		/// <param name="packageViewDTO"></param>
		/// <returns></returns>
		[Authorize(Roles = "Member")]
		[HttpPost]
		[Route("extend-package")]
		public async Task<IActionResult> ExtendPackage(int purchasedPackageId)
		{
			var userDetail = await _userDetailService.GetUserDetailByUserId(UserId);
			if (userDetail == null) return GetError("Vui lòng điền đầy đủ thông tin profile để tiếp tục thanh toán.");
			var package = await _purchasedPackageService.GetUnavailablePackageForMember(purchasedPackageId, UserId);
			if (package == null)
			{
				ModelState.AddModelError("PackageId", "Gói bạn chọn không còn tồn tại.");
				return ModelInvalid();
			}
			var vnpRequest = new VnPayRequestDTO
			{
				UserId = UserId,
				PackageName = package.Name,
				FullName = userDetail.FullName,
				Description = $"{UserId}/{userDetail.FullName} gia han goi {package.Name}. Ma goi {package.Id}",
				Amount = package.Price,
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
				if (response == null) return GetError("Giao dịch này không tồn tại.");
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
			return Ok(new { Url = paymentUrl });
		}

		/// <summary>
		/// This is used to response after payment processing
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("response-payment")]
		public async Task<IActionResult> ResponsePayment()
		{
			var responseUrlFe = _configuration["VnPayConfiguration:ResponseUrlFe"];
			try
			{
				var vnPayResponse = _vnPayService.PaymentExcute(Request.Query);
				if (!vnPayResponse.IsSuccess || !vnPayResponse.VnPayResponseCode.Equals("00"))
				{
					var response = "Loi thanh toan VnPay. Vui long thu lai sau it phut";
					// return RedirectToAction("ResponsePaymentView", new { responseMessage = "Lỗi thanh toán VNpay. Vui lòng thử lại sau ít phút." });
					return Redirect($@"{responseUrlFe}?responseMessage={response}");
				}


				string description = vnPayResponse.OrderDescription;
				string[] parts = description.Split("Ma goi ");
				int packageId = Convert.ToInt32(parts[1]);

				string[] parts1 = description.Split('/');
				string userId = parts1[0];

				var match = Regex.Match(description, @"\b(thanh toan|gia han)\b");

				string action = match.Success ? match.Value : "khong xac dinh";

				var user = await _identityService.GetByIdAsync(userId);
				if (user == null)
					// return RedirectToAction("ResponsePaymentView", new { responseMessage = "Không tìm thấy người dùng." });
					return Redirect($@"{responseUrlFe}?responseMessage=Khong tim thay nguoi dung");


				if (action.Equals("gia han"))
				{
					var extendedPurchasedPackage = await _purchasedPackageService.GetUnavailablePackageForMember(packageId, userId);
					var extendPackage = await _purchasedPackageService.ExtendPurchasedPackage(extendedPurchasedPackage, userId);
					if (!extendPackage.IsSuccess)
						// return RedirectToAction("ResponsePaymentView", new { responseMessage = extendPackage.Message });
						return Redirect($@"{responseUrlFe}?responseMessage=Loi gia han goi.");

					var extendMessage = new EmailDTO
					(
						new string[] { user.Email! },
						"Thông báo phản hồi chuyển khoản.",
						$@"
<p>- Bạn đã hoàn tất gia hạn gói <b>{packageId}</b>.</p>
<p>- Cảm ơn đã sử dụng dịch vụ của chúng tôi.</p>"
					);
					_emailService.SendEmail(extendMessage);
					// return RedirectToAction("ResponsePaymentView", new { responseMessage = Constants.vnp00 });
					return Redirect($@"{responseUrlFe}?responseMessage=Giao dich thanh cong");
				}
				else
				{
					var package = await _advertisementPackageService.GetPackageById(packageId, OrderImage.DatetimeDescending);
					if (package == null)
					{
						var message = new EmailDTO
						(
							new string[] { user.Email! },
							"Thông báo phản hồi chuyển khoản.",
							$@"
<p>- Vì một số nguyên nhân nên gói bạn chọn không còn tồn tại</p>
<p>- Vui lòng phản hồi lại mail này kèm hình ảnh thanh toán để chúng tôi thực hiện việc hoàn tiền cho bạn.</p>
<p>- Chân thành xin lỗi vì sự bất tiện này. Và xin cảm vì đã đồng hành cùng chúng tôi.</p>"
						);
						_emailService.SendEmail(message);
						var response = $"Hệ thống đã gửi mail đến Email: {user.Email}. Xin vui lòng kiểm tra Email của bạn,";
						// return RedirectToAction("ResponsePaymentView", new { responseMessage = response });
						return Redirect($@"{responseUrlFe}?responseMessage=Hay kiem tra Email: {user.Email} cua ban");
					}

					var createdPaymentDTO = new PaymentCreateDTO
					{
						UserId = userId,
						AdvertisementPackageId = packageId,
						TransactionId = vnPayResponse.TransactionId,
						Content = vnPayResponse.OrderDescription,
						Amount = vnPayResponse.Amount / 100,
						CreatedDate = DateTime.Now,
						SelectedPackage = package
					};

					//Snapshort metadata
					createdPaymentDTO.SetMetaDataSnapshot(package);

					var createdPayment = await _paymentService.CreatePayment(createdPaymentDTO);
					if (!createdPayment.IsSuccess)
						// return RedirectToAction("ResponsePaymentView", new { responseMessage = createdPayment.Message });
						return Redirect($@"{responseUrlFe}?responseMessage=Loi tao lich su giao dich.");

					var createdPurchasedPackageDTO = new PurchasedPackageCreateDTO(package, userId);
					var createdPurchasedPackage = await _purchasedPackageService.CreatePurchasedPacakge(createdPurchasedPackageDTO);
					if (!createdPurchasedPackage.IsSuccess)
						// return RedirectToAction("ResponsePaymentView", new { responseMessage = createdPurchasedPackage.Message });
						return Redirect($@"{responseUrlFe}?responseMessage={createdPurchasedPackage.Message}");
					var successMessage = new EmailDTO
										(
											new string[] { user.Email! },
											"Thông báo phản hồi chuyển khoản.",
											$@"
<p>- Bạn đã hoàn tất thanh toán gói <b>{package.Name}</b>.</p>
<p>- Cảm ơn đã sử dụng dịch vụ của chúng tôi.</p>"
										);
					_emailService.SendEmail(successMessage);
					// return RedirectToAction("ResponsePaymentView", new { responseMessage = Constants.vnp00 });
					return Redirect($@"{responseUrlFe}?responseMessage=Giao dich thanh cong.");
				}
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ResetColor();
				return Redirect($@"{responseUrlFe}?responseMessage=Da xay ra loi trong qua trinh xu ly. Vui long thu lai sau.");
			}
		}

		[HttpGet]
		[Route("response-payment-view")]
		public IActionResult ResponsePaymentView(string responseMessage)
		{
			if (string.IsNullOrEmpty(responseMessage)) return GetError();
			return GetSuccess(responseMessage);
		}
	}
}
