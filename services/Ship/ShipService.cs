using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BackEnd.DTOs.Ship;
using BackEnd.Repository;

namespace BackEnd.services.Ship
{
    public class ShipService : IShipService
    {
        private readonly HttpClient _httpClient;
        private readonly IOrderRepository _orderRepository;
        public ShipService(IHttpClientFactory httpClientFactory, IOrderRepository orderRepository)
        {
            _httpClient = httpClientFactory.CreateClient("ghn");
            _orderRepository = orderRepository;
        }

        public async Task<CreateOrderResponse> createOrder(decimal amout, string name, string phone, string address, string wardName, string districtName, string provinceName, string orderCode, long pickUpTime)
        {
            var data = new
            {
                cod_amount = amout,
                to_name = name,
                to_phone = phone,
                to_address = address,
                to_ward_name = wardName,
                to_district_name = districtName,
                to_province_name = provinceName,
                // to_dictrict_id = 1715,
                // to_ward_code = "260414",
                shop_id = 196389,
                required_note = "KHONGCHOXEMHANG",
                payment_type_id = amout == 0 ? 1 : 2,
                client_order_code = orderCode,
                service_type_id = 2,
                length = 30,
                width = 40,
                height = 20,
                weight = 30,
                content = "mua sách",
                pickup_time = pickUpTime,
                // Bổ sung from info
                from_name = "Admin Shop",
                from_phone = "0962970503",
                from_address = "123 Đường ABC",
                from_ward_code = "1B2808",         // mã phường (bắt buộc, đúng với ShopId)
                from_district_id = 3255           // ID quận/huyện gửi (đúng với ShopId)
            };
            var jsonContent = JsonSerializer.Serialize(data);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("https://dev-online-gateway.ghn.vn/shiip/public-api/v2/shipping-order/create", content);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"GHN API Error: {(int)response.StatusCode} - {response.ReasonPhrase}\n{errorContent}");
            }
            var result = await response.Content.ReadFromJsonAsync<CreateOrderResponse>();
            if (result == null)
            {
                throw new Exception("Failed to deserialize response from GHN API.");
            }
            await _orderRepository.Update(orderCode, x => x.TrackingNumber, result.Data!.OrderCode);
            return result;
        }

        public async Task<object> getDistrict(string provinceId)
        {
            var jsonContent = JsonSerializer.Serialize(new { province_id = int.Parse(provinceId) });
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("https://dev-online-gateway.ghn.vn/shiip/public-api/master-data/district", content);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"GHN API Error: {(int)response.StatusCode} - {response.ReasonPhrase}\n{errorContent}");
            }
            var result = await response.Content.ReadFromJsonAsync<object>();
            if (result == null)
            {
                throw new Exception("Failed to deserialize response from GHN API.");
            }
            return result;
        }

        public async Task<object> getFeeShip(object data)
        {
            var jsonContent = JsonSerializer.Serialize(data);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("https://dev-online-gateway.ghn.vn/shiip/public-api/v2/shipping-order/fee", content);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"GHN API Error: {(int)response.StatusCode} - {response.ReasonPhrase}\n{errorContent}");
            }
            var result = await response.Content.ReadFromJsonAsync<object>();
            if (result == null)
            {
                throw new Exception("Failed to deserialize response from GHN API.");
            }
            return result;
        }

        public async Task<object> getProvince()
        {
            var response = await _httpClient.GetAsync("https://dev-online-gateway.ghn.vn/shiip/public-api/master-data/province");
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<object>();
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"GHN API Error: {(int)response.StatusCode} - {response.ReasonPhrase}\n{errorContent}");
            }
            if (result == null)
            {
                throw new Exception("Failed to deserialize response from GHN API.");
            }
            return result;
        }

        public async Task<object> getWard(string districtId)
        {
            var response = await _httpClient.GetAsync($"https://dev-online-gateway.ghn.vn/shiip/public-api/master-data/ward?district_id={districtId}");
            response.EnsureSuccessStatusCode();
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"GHN API Error: {(int)response.StatusCode} - {response.ReasonPhrase}\n{errorContent}");
            }
            var result = await response.Content.ReadFromJsonAsync<object>();

            if (result == null)
            {
                throw new Exception("Failed to deserialize response from GHN API.");
            }
            return result;
        }
        public async Task<ShopResponse> GetShopInfo()
        {
            var response = await _httpClient.GetAsync($"https://dev-online-gateway.ghn.vn/shiip/public-api/v2/shop/all");
            response.EnsureSuccessStatusCode();
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"GHN API Error: {(int)response.StatusCode} - {response.ReasonPhrase}\n{errorContent}");
            }
            var result = await response.Content.ReadFromJsonAsync<ShopResponse>();

            if (result == null)
            {
                throw new Exception("Failed to deserialize response from GHN API.");
            }
            return result;
        }
        public async Task<PrintShipmentResponse> PrintShipment(string orderCode)
        {
            var data = new {order_codes = new List<string> { orderCode } };
            var jsonContent = JsonSerializer.Serialize(data);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"https://dev-online-gateway.ghn.vn/shiip/public-api/v2/a5/gen-token",content);
            response.EnsureSuccessStatusCode();
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"GHN API Error: {(int)response.StatusCode} - {response.ReasonPhrase}\n{errorContent}");
            }
            var result = await response.Content.ReadFromJsonAsync<PrintShipmentResponse>();

            if (result == null)
            {
                throw new Exception("Failed to deserialize response from GHN API.");
            }
            return result;
        }
    }
}