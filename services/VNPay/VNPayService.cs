using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using BackEnd.DTOs.VNPay;
using utils;

namespace BackEnd.services.VNPay
{
    public interface IVNPayService
    {
        string CreatePayMent(VNPayRequest request);
        VNPayResponse VNPayExcute(IQueryCollection query);
    }
    public class VNPayService : IVNPayService
    {
        private readonly IConfiguration _config;
        public VNPayService(IConfiguration configuration)
        {
            _config = configuration;
        }
        public string CreatePayMent(VNPayRequest request)
        {
            Console.WriteLine(request.Amount);
            var locale = _config["VnPay:Locale"];
            var vnp_Url = _config["VnPay:BaseUrl"]!;
            var vnp_HashSecret = _config["VnPay:HashSecret"]!;
            var vnPay = new VnPayLibrary();
            vnPay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnPay.AddRequestData("vnp_Command", _config["VnPay:Command"]!);
            vnPay.AddRequestData("vnp_TmnCode", _config["VnPay:TmnCode"]!);
            vnPay.AddRequestData("vnp_Amount", ((long)request.Amount * 100).ToString()); //Số tiền 
            vnPay.AddRequestData("vnp_CurrCode", _config["VnPay:CurrCode"]!);
            vnPay.AddRequestData("vnp_CreateDate", request.CreatedDate.ToString("yyyyMMddHHmmss"));
            vnPay.AddRequestData("vnp_OrderInfo", HttpUtility.UrlEncode($"Thanhtoandonhang"));
            vnPay.AddRequestData("vnp_OrderType", "other"); //default value: other
            vnPay.AddRequestData("vnp_ReturnUrl", _config["VnPay:PaymentBackReturnUrl"]!);
            vnPay.AddRequestData("vnp_TxnRef",  request.OrderId!.ToString());
            vnPay.AddRequestData("vnp_IpAddr", "127.0.0.1");
            if (!string.IsNullOrEmpty(locale))
            {
                vnPay.AddRequestData("vnp_Locale", locale);
            }
            else
            {
                vnPay.AddRequestData("vnp_Locale", "vn");
            }
            string paymentUrl = vnPay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            return paymentUrl;
        }

        public VNPayResponse VNPayExcute(IQueryCollection query)
        {
            var vnPay = new VnPayLibrary();
            foreach (var (key, value) in query)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnPay.AddResponseData(key, value.ToString());
                }
            }
            var vnp_orderId =vnPay.GetResponseData("vnp_TxnRef");
            var vnp_TransactionId = Convert.ToInt64(vnPay.GetResponseData("vnp_TransactionNo"));
            var vnp_SecureHash = query.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value;
            var vnp_ResponseCode = vnPay.GetResponseData("vnp_ResponseCode");
            var vnp_OrderInfo = vnPay.GetResponseData("vnp_OrderInfo");
            bool checkSignature = vnPay.ValidateSignature(vnp_SecureHash!, _config["VnPay:HashSecret"]!);
            if (!checkSignature)
            {
                return new VNPayResponse
                {
                    Success = false
                };
            }
             return new VNPayResponse
            {
                Success = true,
                PaymentMethod = "VNPay",
                OrderDescription = vnp_OrderInfo,
                OrderId = vnp_orderId.ToString(),
                TransactionId = vnp_TransactionId.ToString(),
                Token = vnp_SecureHash,
                VNPayResponseCode = vnp_ResponseCode
            };
        }
    }
}