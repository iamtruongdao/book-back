using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BackEnd.DTOs.Ship;

namespace BackEnd.services.Ship
{
    public class ShipService : IShipService
    {
        public readonly HttpClient _httpClient;
        public ShipService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ghn");
        }

        public async Task<ShipResponse> createOrder(decimal amout, string name, string phone, string address,string wardName,string districtName,string provinceName, string orderCode)
        {
            var data = new { cod_amount = amout, to_name = name, to_phone = phone, to_address = address, to_ward_name = wardName, to_district_name = districtName, to_province_name = provinceName, required_note = "KHONGCHOXEMHANG", payment_type_id = amout == 0 ? 1 : 2, client_order_code = orderCode, service_type_id = 2 ,length =  30,width = 40,height = 20,weight = 30,content = "mua sách"};
            var jsonContent = JsonSerializer.Serialize(data);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("https://dev-online-gateway.ghn.vn/shiip/public-api/v2/shipping-order/create", content);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"GHN API Error: {(int)response.StatusCode} - {response.ReasonPhrase}\n{errorContent}");
            }
            var result = await response.Content.ReadFromJsonAsync<ShipResponse>();
            if(result == null)
            {
                throw new Exception("Failed to deserialize response from GHN API.");
            }
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
            if(result == null)
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
            if(result == null)
            {
                throw new Exception("Failed to deserialize response from GHN API.");
            }
            return result;
        }

        public async Task<object> getProvince()
        {
            var response = await _httpClient.GetAsync("https://dev-online-gateway.ghn.vn/shiip/public-api/master-data/province" );
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
            var response = await _httpClient.GetAsync($"https://dev-online-gateway.ghn.vn/shiip/public-api/master-data/ward?district_id={districtId}" );
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
    }
}