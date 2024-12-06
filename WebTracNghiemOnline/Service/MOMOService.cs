using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Web;
using WebTracNghiemOnline.DTO;

namespace WebTracNghiemOnline.Service
{
    public interface IMomoService
    {
        Task<MomoCreatePaymentResponseModel> CreatePaymentAsync(string transactionId, decimal amount);
        string GetSecretKey();
        object HandleIpnResponse(JsonElement response);
        MomoExecuteResponseModel HandleReturnUrl(ReturnUrlQuery query);
        bool ValidateSignature(string rawData, string signature);
        public string ComputeHmacSha256(string message, string secretKey);
    }

    public class MomoService : IMomoService
    {
        private readonly IOptions<MomoOptionModel> _options;
        private readonly HttpClient _httpClient;

        public MomoService(IOptions<MomoOptionModel> options, HttpClient httpClient)
        {
            _options = options;
            _httpClient = httpClient;
        }

        /*  public async Task<MomoCreatePaymentResponseModel> CreatePaymentAsync()
          {
              var orderId = DateTime.UtcNow.Ticks.ToString(); // Tạo mã order ngẫu nhiên
              var amount = "15000"; // Số tiền
              var orderInfo = "Thanh toán thử nghiệm MoMo";

              // Kiểm tra cấu hình
              if (string.IsNullOrEmpty(_options.Value.MomoApiUrl) ||
                  string.IsNullOrEmpty(_options.Value.SecretKey) ||
                  string.IsNullOrEmpty(_options.Value.AccessKey))
              {
                  throw new Exception("MoMo configuration is missing or invalid.");
              }

              var rawData =
                  $"partnerCode={_options.Value.PartnerCode}" +
                  $"&accessKey={_options.Value.AccessKey}" +
                  $"&requestId={orderId}" +
                  $"&amount={amount}" +
                  $"&orderId={orderId}" +
                  $"&orderInfo={orderInfo}" +
                  $"&returnUrl={_options.Value.ReturnUrl}" +
                  $"&notifyUrl={_options.Value.NotifyUrl}" +
                  $"&extraData=";

              // Tạo chữ ký
              var signature = ComputeHmacSha256(rawData, _options.Value.SecretKey);

              // Dữ liệu gửi đi
              var requestData = new
              {
                  accessKey = _options.Value.AccessKey,
                  partnerCode = _options.Value.PartnerCode,
                  requestType = _options.Value.RequestType,
                  notifyUrl = _options.Value.NotifyUrl,
                  returnUrl = _options.Value.ReturnUrl,
                  orderId = orderId,
                  amount = amount,
                  orderInfo = orderInfo,
                  requestId = orderId,
                  extraData = "",
                  signature = signature
              };

              // Gửi request đến MoMo
              var jsonData = JsonSerializer.Serialize(requestData);
              var httpRequest = new HttpRequestMessage(HttpMethod.Post, _options.Value.MomoApiUrl)
              {
                  Content = new StringContent(jsonData, Encoding.UTF8, "application/json")
              };

              var httpResponse = await _httpClient.SendAsync(httpRequest);

              if (httpResponse.IsSuccessStatusCode)
              {
                  var responseContent = await httpResponse.Content.ReadAsStringAsync();
                  var momoResponse = JsonSerializer.Deserialize<MomoCreatePaymentResponseModel>(
                      responseContent,
                      new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                  );
                  return momoResponse;
              }
              else
              {
                  throw new Exception($"API Error: {await httpResponse.Content.ReadAsStringAsync()}");
              }
          }
  */
        public string GetSecretKey()
        {
            var secretKey = _options.Value.SecretKey;
            Console.WriteLine($"Retrieved SecretKey: {secretKey}");
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new Exception("SecretKey is null or empty. Check appsettings.json or DI configuration.");
            }
            return secretKey;
        }

        public async Task<MomoCreatePaymentResponseModel> CreatePaymentAsync(string transactionId, decimal amount)
        {
            var orderId = $"{transactionId}-{DateTime.UtcNow.Ticks}";
            var orderInfo = "Nạp tiền tài khoản người dùng";

            var rawData = $"partnerCode={_options.Value.PartnerCode}" +
                          $"&accessKey={_options.Value.AccessKey}" +
                          $"&requestId={orderId}" +
                          $"&amount={amount}" +
                          $"&orderId={orderId}" +
                          $"&orderInfo={orderInfo}" +
                          $"&returnUrl={_options.Value.ReturnUrl}" +
                          $"&notifyUrl={_options.Value.NotifyUrl}" +
                          $"&extraData=";

            var signature = ComputeHmacSha256(rawData, _options.Value.SecretKey);
            Console.WriteLine($"Generated RawData: {rawData}");
            Console.WriteLine($"------------------------Generated Signature---------------------------: {signature}");

            var requestData = new
            {
                partnerCode = _options.Value.PartnerCode,
                accessKey = _options.Value.AccessKey,
                requestId = orderId,
                amount = amount.ToString(),
                orderId = orderId,
                orderInfo = orderInfo,
                returnUrl = _options.Value.ReturnUrl,
                notifyUrl = _options.Value.NotifyUrl,
                extraData = "",
                requestType = _options.Value.RequestType,
                signature = signature
            };

            var jsonData = JsonSerializer.Serialize(requestData);
            Console.WriteLine($"Request Body: {jsonData}");

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, _options.Value.MomoApiUrl)
            {
                Content = new StringContent(jsonData, Encoding.UTF8, "application/json")
            };

            var httpResponse = await _httpClient.SendAsync(httpRequest);

            if (httpResponse.IsSuccessStatusCode)
            {
                var responseContent = await httpResponse.Content.ReadAsStringAsync();
                Console.WriteLine($"Momo API Response: {responseContent}");
                var momoResponse = JsonSerializer.Deserialize<MomoCreatePaymentResponseModel>(
                    responseContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                if (string.IsNullOrEmpty(momoResponse.PayUrl))
                {
                    throw new Exception($"MoMo API did not return a payment URL: {responseContent}");
                }

                return momoResponse;
            }
            else
            {
                var errorContent = await httpResponse.Content.ReadAsStringAsync();
                Console.WriteLine($"Momo API Error Response: {errorContent}");
                throw new Exception($"Momo API Error: {errorContent}");
            }
        }






        public object HandleIpnResponse(JsonElement response)
        {
            try
            {
                // Đọc các thuộc tính từ JsonElement, kiểm tra kiểu dữ liệu
                var partnerCode = response.GetProperty("partnerCode").GetString();
                var orderId = response.GetProperty("orderId").GetString();
                var amount = response.GetProperty("amount").GetRawText(); // Lấy dạng chuỗi, kể cả khi là số
                var resultCode = response.GetProperty("resultCode").GetInt32(); // Lấy kiểu số nguyên
                var message = response.GetProperty("message").GetString();

                // Log dữ liệu để kiểm tra
                Console.WriteLine($"IPN received - OrderId: {orderId}, Amount: {amount}, ResultCode: {resultCode}, Message: {message}");

                // Trả về trạng thái (giả lập luôn là thành công)
                return new { orderId, amount, resultCode, message };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error parsing IPN response: {ex.Message}");
            }
        }



        public MomoExecuteResponseModel HandleReturnUrl(ReturnUrlQuery query)
        {
            if (string.IsNullOrEmpty(query.OrderId) ||
                string.IsNullOrEmpty(query.Amount) ||
                string.IsNullOrEmpty(query.Message))
            {
                throw new Exception("Missing required query parameters");
            }

            Console.WriteLine($"Return URL received - OrderId: {query.OrderId}, Amount: {query.Amount}, Message: {query.Message}");

            return new MomoExecuteResponseModel
            {
                Amount = query.Amount,
                OrderId = query.OrderId,
                OrderInfo = query.Message
            };
        }


        public bool ValidateSignature(string rawData, string signature)
        {
            // Tạo chữ ký kỳ vọng
            var expectedSignature = ComputeHmacSha256(rawData, GetSecretKey().Trim());

            // Log để debug
            Console.WriteLine($"RawData: {rawData}");
            Console.WriteLine($"Signature từ MoMo: {signature}");
            Console.WriteLine($"Expected Signature: {expectedSignature}");

            // So sánh chữ ký
            return signature == expectedSignature;
        }



        public string ComputeHmacSha256(string message, string secretKey)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var messageBytes = Encoding.UTF8.GetBytes(message);

            using (var hmac = new HMACSHA256(keyBytes))
            {
                var hashBytes = hmac.ComputeHash(messageBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower(); // Chuyển thành chữ thường
            }
        }




    }
}
