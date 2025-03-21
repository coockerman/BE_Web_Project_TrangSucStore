using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BEWebPNJ.Models;

namespace BEWebPNJ.Services
{
    public class AddAddressService
    {
        private readonly HttpClient _httpClient;
        private readonly string _firebaseBaseUrl = "https://pnjstore-66a4d-default-rtdb.firebaseio.com/users";

        public AddAddressService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private string GetAddressUrl(string userId, string path = "")
            => $"{_firebaseBaseUrl}/{userId}/addAddress{path}.json";

        // ✅ Lấy danh sách địa chỉ của user (chỉ `addAddress`)
        public async Task<List<AddAddress>> GetUserAddressesAsync(string userId)
        {
            try
            {
                var response = await _httpClient.GetStringAsync(GetAddressUrl(userId));
                if (string.IsNullOrEmpty(response) || response == "null")
                    return new List<AddAddress>();

                var addressesDict = JsonSerializer.Deserialize<Dictionary<string, AddAddress>>(response);
                if (addressesDict == null) return new List<AddAddress>();

                // ✅ Gán id vào từng địa chỉ
                return addressesDict.Select(kv => {
                    kv.Value.id = kv.Key; // Firebase ID là key của object
                    return kv.Value;
                }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy địa chỉ của user {userId}: {ex.Message}");
                return new List<AddAddress>();
            }
        }



        // ✅ Lấy địa chỉ cụ thể theo ID trong `addAddress`
        public async Task<AddAddress?> GetAddressByIdAsync(string userId, string addressId)
        {
            try
            {
                var response = await _httpClient.GetStringAsync(GetAddressUrl(userId, $"/{addressId}"));
                return string.IsNullOrEmpty(response) || response == "null"
                    ? null
                    : JsonSerializer.Deserialize<AddAddress>(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy địa chỉ {addressId} của user {userId}: {ex.Message}");
                return null;
            }
        }

        // ✅ Thêm địa chỉ mới với ID tự động sinh
        public async Task<string?> AddAddressAsync(string userId, AddAddress address)
        {
            try
            {
                var json = JsonSerializer.Serialize(address);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Gửi request POST để Firebase tự tạo ID
                var response = await _httpClient.PostAsync(GetAddressUrl(userId), content);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<Dictionary<string, string>>(responseBody);
                    return result?["name"]; // Firebase trả về ID tự động sinh
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi thêm địa chỉ cho user {userId}: {ex.Message}");
                return null;
            }
        }

        // ✅ Cập nhật địa chỉ dựa trên ID đã có
        public async Task<bool> UpdateAddressAsync(string userId, string addressId, AddAddress address)
        {
            try
            {
                var json = JsonSerializer.Serialize(address);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync(GetAddressUrl(userId, $"/{addressId}"), content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi cập nhật địa chỉ {addressId} của user {userId}: {ex.Message}");
                return false;
            }
        }

        // ✅ Xóa địa chỉ cụ thể khỏi `addAddress`
        public async Task<bool> DeleteAddressAsync(string userId, string addressId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(GetAddressUrl(userId, $"/{addressId}"));
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi xóa địa chỉ {addressId} của user {userId}: {ex.Message}");
                return false;
            }
        }
    }
}
