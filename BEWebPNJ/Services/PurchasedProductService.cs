using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace BEWebPNJ.Services
{
    public class PurchasedService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PurchasedService> _logger;
        private readonly string _firebaseBaseUrl = "https://pnjstore-66a4d-default-rtdb.firebaseio.com/users";

        public PurchasedService(HttpClient httpClient, ILogger<PurchasedService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        private string GetUrl(string userId) => $"{_firebaseBaseUrl}/{userId}/purchasedCart.json";

        // ✅ Lấy danh sách ID sản phẩm đã mua của user
        public async Task<List<string>> GetPurchasedProductsAsync(string userId)
        {
            try
            {
                var response = await _httpClient.GetStringAsync(GetUrl(userId));
                return string.IsNullOrEmpty(response) || response == "null"
                    ? new List<string>()
                    : JsonSerializer.Deserialize<List<string>>(response) ?? new List<string>();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi lấy danh sách purchased của user {userId}: {ex.Message}");
                return new List<string>();
            }
        }

        // ✅ Thêm sản phẩm vào danh sách đã mua
        public async Task<bool> AddPurchasedProductAsync(string userId, string productId)
        {
            try
            {
                var purchasedProducts = await GetPurchasedProductsAsync(userId);
                if (!purchasedProducts.Contains(productId))
                {
                    purchasedProducts.Add(productId);
                }
                var json = JsonSerializer.Serialize(purchasedProducts);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync(GetUrl(userId), content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi thêm purchased sản phẩm {productId} cho user {userId}: {ex.Message}");
                return false;
            }
        }

        // ✅ Xóa sản phẩm khỏi danh sách đã mua
        public async Task<bool> RemovePurchasedProductAsync(string userId, string productId)
        {
            try
            {
                var purchasedProducts = await GetPurchasedProductsAsync(userId);
                if (purchasedProducts.Contains(productId))
                {
                    purchasedProducts.Remove(productId);
                    var json = JsonSerializer.Serialize(purchasedProducts);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await _httpClient.PutAsync(GetUrl(userId), content);
                    return response.IsSuccessStatusCode;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi xóa purchased sản phẩm {productId} của user {userId}: {ex.Message}");
                return false;
            }
        }
    }
}
