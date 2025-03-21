using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BEWebPNJ.Models;
using Microsoft.Extensions.Logging;

namespace BEWebPNJ.Services
{
    public class ShoppingCartService
    {
        private readonly HttpClient _httpClient;
        private readonly string _firebaseBaseUrl = "https://pnjstore-66a4d-default-rtdb.firebaseio.com/users";

        public ShoppingCartService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private string GetUrl(string userId, string path = "")
            => $"{_firebaseBaseUrl}/{userId}/shoppingCart{path}.json";



        public async Task<List<ShoppingCart>> GetUserShoppingCartAsync(string userId)
        {
            try
            {
                var response = await _httpClient.GetStringAsync(GetUrl(userId));
                if (string.IsNullOrEmpty(response) || response == "null")
                    return new List<ShoppingCart>();

                var shoppingCartDirt = JsonSerializer.Deserialize<Dictionary<string, ShoppingCart>>(response);
                if (shoppingCartDirt == null) return new List<ShoppingCart>();

                // ✅ Gán id vào từng địa chỉ
                return shoppingCartDirt.Select(kv => {
                    kv.Value.id = kv.Key; // Firebase ID là key của object
                    return kv.Value;
                }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy gio hang {userId}: {ex.Message}");
                return new List<ShoppingCart>();
            }
        }



        // ✅ Lấy địa chỉ cụ thể theo ID trong `shoppingCart`
        public async Task<ShoppingCart?> GetShoppingCartByIdAsync(string userId, string shoppingCartId)
        {
            try
            {
                var response = await _httpClient.GetStringAsync(GetUrl(userId, $"/{shoppingCartId}"));
                return string.IsNullOrEmpty(response) || response == "null"
                    ? null
                    : JsonSerializer.Deserialize<ShoppingCart>(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy gio hang {shoppingCartId} của user {userId}: {ex.Message}");
                return null;
            }
        }

        // ✅ Thêm cart mới với ID tự động sinh
        public async Task<string?> AddShoppingCartAsync(string userId, ShoppingCart shoppingCart)
        {
            try
            {
                var json = JsonSerializer.Serialize(shoppingCart);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Gửi request POST để Firebase tự tạo ID
                var response = await _httpClient.PostAsync(GetUrl(userId), content);

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
                Console.WriteLine($"Lỗi khi thêm vao gio hang cho user {userId}: {ex.Message}");
                return null;
            }
        }

        // ✅ Cập nhật san pham gio hang dựa trên ID đã có
        public async Task<bool> UpdateShoppingCartAsync(string userId, string shoppingCartId, ShoppingCart shoppingCart)
        {
            try
            {
                var json = JsonSerializer.Serialize(shoppingCart);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync(GetUrl(userId, $"/{shoppingCartId}"), content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi cập nhật shoppingCart {shoppingCartId} của user {userId}: {ex.Message}");
                return false;
            }
        }
        // ✅ Xóa sản phẩm khỏi giỏ hàng theo ID (không cần idProduct & size)
        public async Task<bool> DeleteShoppingCartByIdAsync(string userId, string shoppingCartId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(GetUrl(userId, $"/{shoppingCartId}"));
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi xóa shoppingCart {shoppingCartId} của user {userId}: {ex.Message}");
                return false;
            }
        }

        // ✅ Xóa sản phẩm khỏi giỏ hàng

        public async Task<bool> DeleteShoppingCartAsync(string userId, string idProduct, int size)
        {
            try
            {
                // Lấy danh sách giỏ hàng của user
                var shoppingCarts = await GetUserShoppingCartAsync(userId);
                if (shoppingCarts == null || shoppingCarts.Count == 0)
                    return false;

                // Tìm mục có idProduct và size khớp
                var cartItem = shoppingCarts.FirstOrDefault(cart => cart.idProduct == idProduct && cart.size == size);
                if (cartItem == null)
                    return false;

                // Gửi request DELETE đến Firebase để xóa mục giỏ hàng này
                var response = await _httpClient.DeleteAsync(GetUrl(userId, $"/{cartItem.id}"));
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi xóa sản phẩm {idProduct} (size: {size}) khỏi giỏ hàng của user {userId}: {ex.Message}");
                return false;
            }
        }


    }
}
