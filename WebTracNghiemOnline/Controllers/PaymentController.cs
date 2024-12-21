/*using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WebTracNghiemOnline.DTO;
using WebTracNghiemOnline.Service;

namespace WebTracNghiemOnline.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IMomoService _paymentService;

        public PaymentController(IMomoService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("create-test")]
        public async Task<IActionResult> CreatePaymentTest()
        {
            try
            {
                // Gọi service để tạo thanh toán
                var response = await _paymentService.CreatePaymentAsync();
                return Ok(new { payUrl = response.PayUrl });
            }
            catch (Exception ex)
            {
                // Trả về lỗi chi tiết
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("momo-ipn")]
        public IActionResult IpnHandler([FromBody] JsonElement response)
        {
            try
            {
                // Gọi service xử lý Notify URL
                var result = _paymentService.HandleIpnResponse(response);
                return Ok(new { message = "IPN handled successfully", data = result });
            }
            catch (Exception ex)
            {
                // Trả về lỗi nếu xảy ra
                return BadRequest(new { error = ex.Message });
            }
        }


        [HttpGet("momo-return")]
        public IActionResult ReturnHandler(
     [FromQuery] string orderId,
     [FromQuery] string amount,
     [FromQuery] string resultCode,
     [FromQuery] string message)
        {
            try
            {
                // Tạo một đối tượng giả lập từ các tham số
                var query = new ReturnUrlQuery
                {
                    OrderId = orderId,
                    Amount = amount,
                    ResultCode = resultCode,
                    Message = message
                };

                // Gọi service xử lý Return URL
                var result = _paymentService.HandleReturnUrl(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


    }
}
*/
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using System.Web;
using WebTracNghiemOnline.DTO;
using WebTracNghiemOnline.Service;

namespace WebTracNghiemOnline.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    /*[Authorize]*/
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }
        [HttpPost("deposit")]
        public async Task<IActionResult> CreateDeposit([FromBody] DepositRequestDTO request)
        {
            try
            {
                // Lấy token từ cookie
                var token = Request.Cookies["jwt"];
                if (string.IsNullOrEmpty(token))
                {
                    return Unauthorized(new { error = "Token is missing or invalid." });
                }
                //Đoạn này có thể lấy user để xử lý ở đây
                // Gọi service để tạo giao dịch vấn đề ở đây là chỉ nhận token
                var paymentUrl = await _paymentService.CreateDepositAsync(token, request.Amount);

                return Ok(new { paymentUrl });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }



        [HttpPost("momo-ipn")]
        public async Task<IActionResult> HandleNotify([FromBody] JsonElement response)
        {
            try
            {
                await _paymentService.HandleNotifyAsync(response);
                return Ok(new { message = "IPN handled successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        [HttpGet("momo-return")]
        public async Task<IActionResult> HandleReturnUrl(
    [FromQuery] string partnerCode,
    [FromQuery] string accessKey,
    [FromQuery] string requestId,
    [FromQuery] string amount,
    [FromQuery] string orderId,
    [FromQuery] string orderInfo,
    [FromQuery] string transId,
    [FromQuery] string message,
    [FromQuery] string localMessage,
    [FromQuery] string errorCode,
    [FromQuery] string payType,
    [FromQuery] string signature)
        {
            try
            {
                // Giải mã các tham số
                orderInfo = HttpUtility.UrlDecode(orderInfo ?? string.Empty);
                message = HttpUtility.UrlDecode(message ?? string.Empty);
                localMessage = HttpUtility.UrlDecode(localMessage ?? string.Empty);

                // Log dữ liệu để debug
                Console.WriteLine($"Decoded orderInfo: {orderInfo}");
                Console.WriteLine($"Decoded message: {message}");
                Console.WriteLine($"Decoded localMessage: {localMessage}");

                // Gọi service xử lý return URL
                var result = await _paymentService.HandleReturnAsync(
                    partnerCode, accessKey, requestId, amount, orderId, orderInfo,
                    transId, message, localMessage, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                    errorCode, payType, string.Empty, signature);

                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                // Log lỗi
                Console.WriteLine($"Error in HandleReturnUrl: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
        }



    }
}
