using System.Text.Json;
using System.Web;
using WebTracNghiemOnline.Models;
using WebTracNghiemOnline.Repository;
using WebTracNghiemOnline.Services;

namespace WebTracNghiemOnline.Service
{
    public interface IPaymentService
    {
        Task<string> CreateDepositAsync(string token, decimal amount);
        Task HandleNotifyAsync(JsonElement response);
        Task<string> HandleReturnAsync(
      string partnerCode,
      string accessKey,
      string requestId,
      string amount,
      string orderId,
      string orderInfo,
      string transId,
      string message,
      string localMessage,
      string responseTime,
      string errorCode,
      string payType,
      string extraData,
      string signature);
    }

    public class PaymentService : IPaymentService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMomoService _momoService;
        private readonly IUserRepository _userRepository;
        private readonly IAuthService _authService;


        public PaymentService(
            ITransactionRepository transactionRepository,
            IMomoService momoService,
            IUserRepository userRepository,
            IAuthService authService)
        {
            _transactionRepository = transactionRepository;
            _momoService = momoService;
            _userRepository = userRepository;
            _authService = authService;
        }

        public async Task<string> CreateDepositAsync(string token, decimal amount)
        {
            if (amount < 10000 || amount > 500000)
            {
                throw new Exception("Số tiền nạp phải từ 10,000 đến 500,000 VND.");
            }

            // Lấy thông tin người dùng từ token
            var user = await _authService.ValidateTokenAsync(token);
            if (user == null)
            {
                throw new Exception("Người dùng không tồn tại hoặc token không hợp lệ.");
            }

            // Tạo giao dịch mới
            var transaction = new Transaction
            {
                UserId = user.Id,
                Amount = amount,
                TransactionDate = DateTime.Now,
                Status = "Pending"
            };

            transaction = await _transactionRepository.CreateTransactionAsync(transaction);

            // Gọi MoMo để tạo thanh toán
            var paymentResponse = await _momoService.CreatePaymentAsync(transaction.TransactionId, amount);
            Console.WriteLine($"Momo API Response: {paymentResponse}");
            return paymentResponse.PayUrl; // Trả về URL để người dùng thanh toán
        }

        public async Task HandleNotifyAsync(JsonElement response)
        {
            try
            {
                var orderId = response.GetProperty("orderId").GetString(); // Lấy dưới dạng string

                var resultCode = response.GetProperty("resultCode").GetInt32();

                // Lấy giao dịch từ DB
                var transaction = await _transactionRepository.GetTransactionByIdAsync(orderId);
                if (transaction == null)
                {
                    throw new Exception($"Transaction with OrderId {orderId} not found.");
                }

                if (resultCode == 0) // Thanh toán thành công
                {
                    transaction.Status = "Success";
                }
                else if (resultCode == 49) // Người dùng hủy giao dịch
                {
                    transaction.Status = "Cancelled";
                }
                else // Lỗi khác
                {
                    transaction.Status = "Failed";
                }

                // Cập nhật trạng thái giao dịch
                await _transactionRepository.UpdateTransactionStatusAsync(transaction.TransactionId, transaction.Status);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling IPN: {ex.Message}");
                throw;
            }
        }
        public async Task<string> HandleReturnAsync(
      string partnerCode,
      string accessKey,
      string requestId,
      string amount,
      string orderId,
      string orderInfo,
      string transId,
      string message,
      string localMessage,
      string responseTime,
      string errorCode,
      string payType,
      string extraData,
      string signature)
        {
            try
            {
                // Mã hóa URL các tham số có thể chứa ký tự đặc biệt
                orderInfo = HttpUtility.UrlEncode(orderInfo ?? string.Empty);
                message = HttpUtility.UrlEncode(message ?? string.Empty);
                localMessage = HttpUtility.UrlEncode(localMessage ?? string.Empty);
                extraData = HttpUtility.UrlEncode(extraData ?? string.Empty);
                Console.WriteLine($"------------------------------------------");
                Console.WriteLine($"Encoded orderInfo: {HttpUtility.UrlEncode(orderInfo)}");
                Console.WriteLine($"------------------------------------------");
                Console.WriteLine($"Encoded message: {HttpUtility.UrlEncode(message)}");
                Console.WriteLine($"------------------------------------------");
                Console.WriteLine($"Encoded localMessage: {HttpUtility.UrlEncode(localMessage)}");
                Console.WriteLine($"------------------------------------------");
                // Tạo lại rawData
                var rawData = $"partnerCode={partnerCode}&accessKey={accessKey}&requestId={requestId}&amount={amount}" +
              $"&orderId={orderId}&orderInfo={HttpUtility.UrlEncode(orderInfo)}&orderType=momo_wallet" +
              $"&transId={transId}&message={HttpUtility.UrlEncode(message)}&localMessage={HttpUtility.UrlEncode(localMessage)}" +
              $"&responseTime={responseTime}&errorCode={errorCode}&payType={payType}&extraData={HttpUtility.UrlEncode(extraData)}";

                Console.WriteLine($"RawData: {rawData}");
                Console.WriteLine($"------------------------------------------");


                // Xác thực chữ ký
                var expectedSignature = _momoService.ComputeHmacSha256(rawData, _momoService.GetSecretKey());
                Console.WriteLine($"Expected Signature: {expectedSignature}");
                Console.WriteLine($"------------------------------------------");

                Console.WriteLine($"Signature from MoMo: {signature}");
                Console.WriteLine($"------------------------------------------");


                if (signature != expectedSignature)
                {
                    throw new Exception("Invalid signature.");
                }

                // Kiểm tra trạng thái giao dịch
                if (errorCode != "0")
                {
                    var transaction = await _transactionRepository.GetTransactionByIdAsync(orderId);
                    if (transaction == null)
                    {
                        throw new Exception($"Transaction with OrderId {orderId} not found.");
                    }

                    transaction.Status = "Success";
                    await _transactionRepository.UpdateTransactionStatusAsync(transaction.TransactionId, transaction.Status);

                    return "Thanh toán thành công!";
                }
                else
                {
                    throw new Exception($"Giao dịch thất bại: {localMessage} (ErrorCode: {errorCode})");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error handling return: {ex.Message}");
            }
        }





    }
}
