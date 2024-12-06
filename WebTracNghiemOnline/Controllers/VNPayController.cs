using Microsoft.AspNetCore.Mvc;
using WebTracNghiemOnline.DTO;
using WebTracNghiemOnline.Service;

namespace WebTracNghiemOnline.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VNPayController : ControllerBase
    {
        private readonly IVNPAYService _vnPayService;

        public VNPayController(IVNPAYService vnPayService)
        {
            _vnPayService = vnPayService;
        }

        [HttpPost("create-payment")]
        public IActionResult CreatePaymentUrl([FromBody] PaymentInformationModel model)
        {
            try
            {
                var token = Request.Cookies["jwt"];
                if (string.IsNullOrEmpty(token))
                {
                    throw new Exception("Token is missing or invalid.");
                }

                var url = _vnPayService.CreatePaymentUrl(model, HttpContext);
                return Ok(new { paymentUrl = url });
            }
            catch (Exception ex)
            {
                // Log lỗi
                Console.WriteLine($"Error: {ex.Message}");
                return BadRequest(new { error = ex.Message });
            }
        }



        [HttpGet("vnpay-return")]
        public async Task<IActionResult> PaymentCallbackVnpay()
        {
            try
            {
                // Xử lý callback từ VNPay
                var response = await _vnPayService.PaymentExecute(Request.Query);

                // Kiểm tra nếu phản hồi không hợp lệ
                if (response == null || !response.Success)
                {
                    // Trả về kết quả thất bại, redirect đến giao diện frontend
                    return Redirect($"https://localhost:5173/payment-result?success=false&message={Uri.EscapeDataString(response?.ErrorMessage ?? "Thanh toán không thành công")}");
                }

                // Trả về kết quả thành công, redirect đến giao diện frontend
                return Redirect($"https://localhost:5173/payment-result?success=true&message={Uri.EscapeDataString("Thanh toán thành công!")}");
            }
            catch (Exception ex)
            {
                // Log lỗi
                return Redirect($"https://localhost:5173/payment-result?success=false&message={Uri.EscapeDataString("Có lỗi xảy ra trong quá trình thanh toán.")}");
            }
        }


    }
}
