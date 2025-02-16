using System;
using System.Collections.Generic;
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
        private string _firebaseBaseUrl = $"https://pnjstore-66a4d-default-rtdb.firebaseio.com/authentication";

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private string GetUrl(string path) => $"{_firebaseBaseUrl}/{path}.json";

        // ✅ Lấy danh sách tất cả người dùng
        public async Task<List<User>> GetAllUsersAsync()
        {
            var response = await _httpClient.GetStringAsync(GetUrl(""));
            if (string.IsNullOrEmpty(response)) return new List<User>();

            var usersDict = JsonSerializer.Deserialize<Dictionary<string, User>>(response);
            return usersDict != null ? new List<User>(usersDict.Values) : new List<User>();
        }

        // ✅ Lấy thông tin người dùng theo ID
        public async Task<User?> GetUserByIdAsync(string id)
        {
            var response = await _httpClient.GetStringAsync(GetUrl(id));
            return string.IsNullOrEmpty(response) || response == "null" ? null : JsonSerializer.Deserialize<User>(response);
        }

        // ✅ Thêm hoặc cập nhật thông tin người dùng
        public async Task<bool> SetUserAsync(User user)
        {
            var json = JsonSerializer.Serialize(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(GetUrl(user.Id), content);
            return response.IsSuccessStatusCode;
        }

        // ✅ Cập nhật một số trường của user
        public async Task<bool> UpdateUserAsync(string id, Dictionary<string, object> updates)
        {
            var json = JsonSerializer.Serialize(updates);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PatchAsync(GetUrl(id), content);
            return response.IsSuccessStatusCode;
        }

        // ✅ Xóa người dùng theo ID
        public async Task<bool> DeleteUserAsync(string id)
        {
            var response = await _httpClient.DeleteAsync(GetUrl(id));
            return response.IsSuccessStatusCode;
        }
    }
}
