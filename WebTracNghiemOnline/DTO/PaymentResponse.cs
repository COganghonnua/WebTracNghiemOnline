namespace WebTracNghiemOnline.DTO
{
    public class PaymentResponse
    {
        public string vnp_TmnCode { get; set; } // Mã terminal của merchant
        public string vnp_Amount { get; set; } // Số tiền thanh toán
        public string vnp_BankCode { get; set; } // Mã ngân hàng
        public string vnp_BankTranNo { get; set; } // Mã giao dịch ngân hàng
        public string vnp_CardType { get; set; } // Loại thẻ
        public string vnp_PayDate { get; set; } // Ngày thanh toán
        public string vnp_OrderInfo { get; set; } // Thông tin đơn hàng
        public string vnp_TransactionNo { get; set; } // Mã giao dịch VNPAY
        public string vnp_ResponseCode { get; set; } // Mã phản hồi (00: thành công)
        public string vnp_TxnRef { get; set; } // Mã tham chiếu giao dịch
        public string vnp_SecureHash { get; set; } // Chuỗi hash để kiểm tra
    }
}
