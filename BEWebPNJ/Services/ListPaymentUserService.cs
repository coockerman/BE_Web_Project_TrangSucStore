using System.Text;
using System.Text.Json;

namespace BEWebPNJ.Services
{
    public class ListPaymentUserService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PurchasedService> _logger;
        private readonly string _firebaseBaseUrl = "https://pnjstore-66a4d-default-rtdb.firebaseio.com/users";

        public ListPaymentUserService(HttpClient httpClient, ILogger<PurchasedService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        private string GetUrl(string userId) => $"{_firebaseBaseUrl}/{userId}/purchasedCart.json";

        // ✅ Lấy danh sách order đã mua của user
        public async Task<List<string>> GetOrderUserAsync(string userId)
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
                _logger.LogError($"Lỗi khi lấy danh sách order của user {userId}: {ex.Message}");
                return new List<string>();
            }
        }

        // ✅ Thêm sản phẩm vào danh sách đã mua
        public async Task<bool> AddOrderUserAsync(string userId, string orderId)
        {
            try
            {
                var purchasedProducts = await GetOrderUserAsync(userId);
                if (!purchasedProducts.Contains(orderId))
                {
                    purchasedProducts.Add(orderId);
                }
                var json = JsonSerializer.Serialize(purchasedProducts);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync(GetUrl(userId), content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi thêm order {orderId} cho user {userId}: {ex.Message}");
                return false;
            }
        }
    }
}
