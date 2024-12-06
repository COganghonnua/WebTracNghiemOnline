namespace WebTracNghiemOnline.DTO
{
    public class PaymentInformationModel
    {
        public string? OrderType { get; set; } = "NT"; // Loại đơn hàng =
        public decimal Amount { get; set; }  // Số tiền
        public string ? OrderDescription { get; set; } // Mô tả đơn hàng
        public string? CustomerName { get; set; } // Tên khách hàng
    }
    public class PaymentResponseModel
    {
        public bool Success { get; set; }  // Thành công hay không
        public string ErrorMessage { get; set; }  // Thông báo lỗi nếu có
        public string OrderDescription { get; set; } // Thông tin đơn hàng
        public string PaymentId { get; set; } // Mã giao dịch VNPAY
        public string OrderId { get; set; } // Mã tham chiếu giao dịch
        public string VnPayResponseCode { get; set; } // Mã phản hồi
    }


}
