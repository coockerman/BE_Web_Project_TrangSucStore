using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace BEWebPNJ.Services
{
    public class ShoppingCartService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ShoppingCartService> _logger;
        private readonly string _firebaseBaseUrl = "https://pnjstore-66a4d-default-rtdb.firebaseio.com/users";

        public ShoppingCartService(HttpClient httpClient, ILogger<ShoppingCartService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        private string GetUrl(string userId) => $"{_firebaseBaseUrl}/{userId}/shoppingCart.json";
        private string GetProductUrl(string userId, string productId) => $"{_firebaseBaseUrl}/{userId}/shoppingCart/{productId}.json";

        // ✅ Lấy giỏ hàng của user
        public async Task<Dictionary<string, Dictionary<string, object>>> GetShoppingCartAsync(string userId)
        {
            try
            {
                var response = await _httpClient.GetStringAsync(GetUrl(userId));
                return string.IsNullOrEmpty(response) || response == "null"
                    ? new Dictionary<string, Dictionary<string, object>>()
                    : JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, object>>>(response) ?? new Dictionary<string, Dictionary<string, object>>();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi lấy giỏ hàng của user {userId}: {ex.Message}");
                return new Dictionary<string, Dictionary<string, object>>();
            }
        }

        // ✅ Thêm sản phẩm vào giỏ hàng
        public async Task<bool> AddToShoppingCartAsync(string userId, string productId, Dictionary<string, object> productData)
        {
            try
            {
                var json = JsonSerializer.Serialize(productData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync(GetProductUrl(userId, productId), content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi thêm sản phẩm {productId} vào giỏ hàng của user {userId}: {ex.Message}");
                return false;
            }
        }

        // ✅ Xóa sản phẩm khỏi giỏ hàng
        public async Task<bool> RemoveFromShoppingCartAsync(string userId, string productId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(GetProductUrl(userId, productId));
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi xóa sản phẩm {productId} khỏi giỏ hàng của user {userId}: {ex.Message}");
                return false;
            }
        }
    }
}
