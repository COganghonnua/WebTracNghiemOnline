using System.Net;
using System.Security.Cryptography;
using System.Text;
using WebTracNghiemOnline.DTO;
using WebTracNghiemOnline.VnPayLibrary;
namespace WebTracNghiemOnline.VnPayLibrary
{
    public class VnPayLibrary
    {
        private readonly SortedList<string, string> _requestData = new SortedList<string, string>();
        private readonly SortedList<string, string> _responseData = new SortedList<string, string>();

        public void AddRequestData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _requestData.Add(key, value);
            }
        }

        public string CreateRequestUrl(string baseUrl, string vnpHashSecret)
        {
            var data = new StringBuilder();
            foreach (var (key, value) in _requestData)
            {
                data.Append(WebUtility.UrlEncode(key) + "=" + WebUtility.UrlEncode(value) + "&");
            }

            var querystring = data.ToString();
            querystring = querystring.TrimEnd('&');
            var vnpSecureHash = HmacSha512(vnpHashSecret, querystring);
            return $"{baseUrl}?{querystring}&vnp_SecureHash={vnpSecureHash}";
        }

        public PaymentResponseModel GetFullResponseData(IQueryCollection collection, string hashSecret)
        {
            foreach (var (key, value) in collection)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    _responseData[key] = value;
                }
            }

            var rawData = GetRawResponseData();
            var inputHash = collection["vnp_SecureHash"];
            var computedHash = HmacSha512(hashSecret, rawData);

            // Log chi tiết
            Console.WriteLine($"RawData: {rawData}");
            Console.WriteLine($"InputHash (VNPay): {inputHash}");
            Console.WriteLine($"ComputedHash (Server): {computedHash}");

            if (!computedHash.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase))
            {
                return new PaymentResponseModel
                {
                    Success = false,
                    ErrorMessage = "Signature validation failed."
                };
            }

            return new PaymentResponseModel
            {
                Success = true,
                OrderDescription = _responseData.GetValueOrDefault("vnp_OrderInfo"),
                PaymentId = _responseData.GetValueOrDefault("vnp_TransactionNo"),
                OrderId = _responseData.GetValueOrDefault("vnp_TxnRef"),
                VnPayResponseCode = _responseData.GetValueOrDefault("vnp_ResponseCode")
            };
        }




        public bool ValidateSignature(string inputHash, string secretKey)
        {
            var data = GetRawResponseData();
            var computedHash = HmacSha512(secretKey, data);
            return computedHash.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
        }

        private string GetRawResponseData()
        {
            // Sắp xếp tham số theo thứ tự từ điển
            var sortedParams = _responseData
                .OrderBy(x => x.Key)
                .ToDictionary(x => x.Key, x => x.Value);

            // Loại bỏ các tham số không cần thiết
            sortedParams.Remove("vnp_SecureHash");
            sortedParams.Remove("vnp_SecureHashType");

            // Tạo chuỗi key=value&key2=value2 mà không URL decode
            return string.Join("&", sortedParams.Select(kvp => $"{kvp.Key}={WebUtility.UrlEncode(kvp.Value)}"));
        }



        private string HmacSha512(string key, string inputData)
        {
            using (var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key)))
            {
                return BitConverter.ToString(hmac.ComputeHash(Encoding.UTF8.GetBytes(inputData)))
                    .Replace("-", "")
                    .ToUpper(); // Sử dụng UpperCase để khớp với VNPay
            }
        }


        public string GetIpAddress(HttpContext context)
        {
            return context.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
        }

    }

}
