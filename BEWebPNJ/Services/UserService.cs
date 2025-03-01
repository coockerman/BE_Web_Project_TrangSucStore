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
    public class UserService
    {
        private readonly HttpClient _httpClient;
        private readonly string _firebaseBaseUrl = "https://pnjstore-66a4d-default-rtdb.firebaseio.com/users";

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private string GetUrl(string path) => $"{_firebaseBaseUrl}/{path}.json";

        // ✅ Lấy danh sách tất cả người dùng
        public async Task<List<User>> GetAllUsersAsync()
        {
            try
            {
                var response = await _httpClient.GetStringAsync(GetUrl(""));
                if (string.IsNullOrEmpty(response) || response == "null") return new List<User>();

                var usersDict = JsonSerializer.Deserialize<Dictionary<string, User>>(response);
                if (usersDict == null) return new List<User>();

                // ✅ Gán ID từ key vào object User
                return usersDict.Select(user =>
                {
                    user.Value.id = user.Key; // Gán ID từ key của dictionary
                    return user.Value;
                }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy danh sách user: {ex.Message}");
                return new List<User>();
            }
        }


        // ✅ Lấy thông tin người dùng theo ID
        public async Task<User?> GetUserByIdAsync(string id)
        {
            try
            {
                var response = await _httpClient.GetStringAsync(GetUrl(id));
                return string.IsNullOrEmpty(response) || response == "null" ? null : JsonSerializer.Deserialize<User>(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy user ID {id}: {ex.Message}");
                return null;
            }
        }

        // ✅ Thêm hoặc cập nhật thông tin người dùng
        public async Task<bool> SetUserAsync(User user)
        {
            try
            {
                var json = JsonSerializer.Serialize(user);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync(GetUrl(user.id), content);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Lỗi khi thêm/cập nhật user {user.id}: {response.ReasonPhrase}");
                }
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi thêm/cập nhật user {user.id}: {ex.Message}");
                return false;
            }
        }

        // ✅ Cập nhật một số trường của user
        public async Task<bool> UpdateUserAsync(string id, Dictionary<string, object> updates)
        {
            if (updates == null || updates.Count == 0) return false;

            try
            {
                var json = JsonSerializer.Serialize(updates);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PatchAsync(GetUrl(id), content);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Lỗi cập nhật user {id}: {response.ReasonPhrase}");
                }
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi cập nhật user {id}: {ex.Message}");
                return false;
            }
        }

        // ✅ Xóa người dùng theo ID
        public async Task<bool> DeleteUserAsync(string id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(GetUrl(id));
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Lỗi khi xóa user {id}: {response.ReasonPhrase}");
                }
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi xóa user {id}: {ex.Message}");
                return false;
            }
        }
    }
}
