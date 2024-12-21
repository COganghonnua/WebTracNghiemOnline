using System.Security.Cryptography;
using System.Text;
using WebTracNghiemOnline.DTO;
using WebTracNghiemOnline.Models;
using WebTracNghiemOnline.Repository;
using WebTracNghiemOnline.Services;

namespace WebTracNghiemOnline.Service
{
    public interface IVNPAYService
    {
        string CreatePaymentUrl(PaymentInformationModel model, HttpContext context);
        Task<PaymentResponseModel> PaymentExecute(IQueryCollection collections);
    }
    public class VNPAYService : IVNPAYService
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthService _authService;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IUserRepository _userRepository;

        public VNPAYService(IConfiguration configuration, IAuthService authService, ITransactionRepository transactionRepository, IUserRepository userRepository)
        {
            _configuration = configuration;
            _authService = authService;
            _transactionRepository = transactionRepository;
            _userRepository = userRepository;
        }

        public string CreatePaymentUrl(PaymentInformationModel model, HttpContext context)
        {
            // Lấy thông tin người dùng từ token
            var token = context.Request.Cookies["jwt"];
            if (string.IsNullOrEmpty(token))
            {
                throw new Exception("Token is missing or invalid.");
            }

            var user = _authService.ValidateTokenAsync(token).Result;
            if (user == null)
            {
                throw new Exception("User not found or invalid token.");
            }

            // Khởi tạo thông tin giao dịch
            var transactionId = Guid.NewGuid().ToString(); // Tạo TransactionId (vnp_TxnRef)
            var transaction = new Transaction
            {
                TransactionId = transactionId,
                UserId = user.Id,
                Amount = model.Amount,
                Status = "Pending",
                TransactionDate = DateTime.UtcNow
            };

            // Lưu giao dịch vào cơ sở dữ liệu
            _transactionRepository.CreateTransactionAsync(transaction).Wait();

            // Tạo URL thanh toán
            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]);
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
            Console.WriteLine("---------------------------");
            Console.WriteLine("Time now là gì : ", timeNow.ToString("yyyyMMddHHmmss"));
            Console.WriteLine("---------------------------");
            var pay = new WebTracNghiemOnline.VnPayLibrary.VnPayLibrary();
            pay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
            pay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
            pay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
            pay.AddRequestData("vnp_Amount", ((int)(model.Amount * 100)).ToString()); // bắt buộc lấy * 100.
            pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]);
            pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
            pay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]);
            pay.AddRequestData("vnp_OrderInfo", $"{user.UserName} Nạp tiền");
            pay.AddRequestData("vnp_OrderType", model.OrderType);
            pay.AddRequestData("vnp_ReturnUrl", _configuration["Vnpay:PaymentBackReturnUrl"]);
            pay.AddRequestData("vnp_TxnRef", transactionId); // Sử dụng TransactionId làm vnp_TxnRef

            Console.WriteLine("Rawdata khi tra ve URL Thanh toán : ", pay.CreateRequestUrl(_configuration["Vnpay:BaseUrl"], _configuration["Vnpay:HashSecret"]));
            return pay.CreateRequestUrl(_configuration["Vnpay:BaseUrl"], _configuration["Vnpay:HashSecret"]);
        }

        public async Task<PaymentResponseModel> PaymentExecute(IQueryCollection collections)
        {
            var pay = new WebTracNghiemOnline.VnPayLibrary.VnPayLibrary();

            // Kiểm tra chữ ký hợp lệ
            var response = pay.GetFullResponseData(collections, _configuration["Vnpay:HashSecret"]);
            if (!response.Success)
            {
                return new PaymentResponseModel
                {
                    Success = false,
                    ErrorMessage = "Signature validation failed."
                };
            }

            // Lấy TransactionId từ vnp_TxnRef
            var transactionId = collections["vnp_TxnRef"];
            var transaction = await _transactionRepository.GetTransactionByIdAsync(transactionId);

            if (transaction == null)
            {
                return new PaymentResponseModel
                {
                    Success = false,
                    ErrorMessage = "Transaction not found."
                }; 
            }

            // Cập nhật trạng thái giao dịch
            transaction.Status = collections["vnp_ResponseCode"] == "00" ? "Success" : "Fail";
            transaction.TransactionDate = DateTime.Now;

            // Cập nhật số dư người dùng nếu thành công
            if (transaction.Status == "Success")
            {
                var user = await _userRepository.GetByIdAsync(transaction.UserId);
                if (user != null)
                {
                    user.Balance += transaction.Amount;
                    await _userRepository.UpdateAsync(user);
                }
            }

            // Lưu thay đổi giao dịch
            await _transactionRepository.UpdateTransactionStatusAsync(transaction.TransactionId, transaction.Status);

            return new PaymentResponseModel
            {
                Success = transaction.Status == "Success",
                OrderDescription = collections["vnp_OrderInfo"],
                PaymentId = collections["vnp_TransactionNo"],
                OrderId = transactionId,
                VnPayResponseCode = collections["vnp_ResponseCode"]
            };
        }




    }

}
