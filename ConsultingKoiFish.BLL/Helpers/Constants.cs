using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.BLL.Helpers
{
	public class Constants
	{
		#region VnPayResponseCode

		public const string vnp00 = "Giao dịch thành công";
		public const string vnp07 = "Trừ tiền thành công. Giao dịch bị nghi ngờ (liên quan tới lừa đảo, giao dịch bất thường).";
		public const string vnp09 = "Giao dịch không thành công do: Thẻ/Tài khoản của khách hàng chưa đăng ký dịch vụ InternetBanking tại ngân hàng.";
		public const string vnp10 = "Giao dịch không thành công do: Khách hàng xác thực thông tin thẻ/tài khoản không đúng quá 3 lần";
		public const string vnp11 = "Giao dịch không thành công do: Đã hết hạn chờ thanh toán. Xin quý khách vui lòng thực hiện lại giao dịch.";
		public const string vnp12 = "Giao dịch không thành công do: Thẻ/Tài khoản của khách hàng bị khóa.";
		public const string vnp13 = "Giao dịch không thành công do Quý khách nhập sai mật khẩu xác thực giao dịch (OTP). Xin quý khách vui lòng thực hiện lại giao dịch.";
		public const string vnp24 = "Giao dịch không thành công do: Khách hàng hủy giao dịch";
		public const string vnp51 = "Giao dịch không thành công do: Tài khoản của quý khách không đủ số dư để thực hiện giao dịch.";
		public const string vnp65 = "Giao dịch không thành công do: Tài khoản của Quý khách đã vượt quá hạn mức giao dịch trong ngày.";
		public const string vnp75 = "Ngân hàng thanh toán đang bảo trì.";
		public const string vnp79 = "Giao dịch không thành công do: KH nhập sai mật khẩu thanh toán quá số lần quy định. Xin quý khách vui lòng thực hiện lại giao dịch";
		public const string vnp99 = "Các lỗi khác (lỗi còn lại, không có trong danh sách mã lỗi đã liệt kê)";

		#endregion
	}
}

