using System.Text;
using System.Text.Json;


namespace BEWebPNJ.Services
{
    public class FavouriteProductService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<FavouriteProductService> _logger;
        private readonly string _firebaseBaseUrl = "https://pnjstore-66a4d-default-rtdb.firebaseio.com/users";

        public FavouriteProductService(HttpClient httpClient, ILogger<FavouriteProductService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        private string GetUrl(string userId) => $"{_firebaseBaseUrl}/{userId}/favouriteCart.json";

        // ✅ Lấy danh sách ID sản phẩm yêu thích của user
        public async Task<List<string>> GetFavouriteProductsAsync(string userId)
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
                _logger.LogError($"Lỗi khi lấy danh sách favourite của user {userId}: {ex.Message}");
                return new List<string>();
            }
        }

        // ✅ Thêm sản phẩm vào danh sách yêu thích
        public async Task<bool> AddFavouriteProductAsync(string userId, string productId)
        {
            try
            {
                var favouriteProducts = await GetFavouriteProductsAsync(userId);
                if (!favouriteProducts.Contains(productId))
                {
                    favouriteProducts.Add(productId);
                }
                var json = JsonSerializer.Serialize(favouriteProducts);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync(GetUrl(userId), content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi thêm favourite sản phẩm {productId} cho user {userId}: {ex.Message}");
                return false;
            }
        }

        // ✅ Xóa sản phẩm khỏi danh sách yêu thích
        public async Task<bool> RemoveFavouriteProductAsync(string userId, string productId)
        {
            try
            {
                var favouriteProducts = await GetFavouriteProductsAsync(userId);
                if (favouriteProducts.Contains(productId))
                {
                    favouriteProducts.Remove(productId);
                    var json = JsonSerializer.Serialize(favouriteProducts);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await _httpClient.PutAsync(GetUrl(userId), content);
                    return response.IsSuccessStatusCode;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi xóa favourite sản phẩm {productId} của user {userId}: {ex.Message}");
                return false;
            }
        }
    }
}


