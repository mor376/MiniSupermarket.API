using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace MiniSupermarket.WinForms
{
    public static class ApiClientService
    {
        // Ensure this matches your API URL (use the HTTPS port from launchSettings.json)
        private static readonly HttpClient _client = new HttpClient { BaseAddress = new Uri("https://localhost:7035/api/") };

        public static async Task<bool> LoginAsync(string username, string password)
        {
            var loginObj = new { Username = username, Password = password };
            var response = await _client.PostAsJsonAsync("auth/login", loginObj);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(jsonString);
                SessionManager.JwtToken = doc.RootElement.GetProperty("token").GetString() ?? string.Empty;
                SessionManager.CurrentRole = doc.RootElement.GetProperty("role").GetString() ?? string.Empty;
                return true;
            }
            return false;
        }

        private static void AttachToken()
        {
            if (!string.IsNullOrEmpty(SessionManager.JwtToken))
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", SessionManager.JwtToken);
            }
            else
            {
                _client.DefaultRequestHeaders.Authorization = null;
            }
        }

        public static async Task<T?> GetAsync<T>(string endpoint)
        {
            AttachToken();
            var response = await _client.GetAsync(endpoint);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<T>();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new Exception("Phiên làm việc hết hạn hoặc chưa đăng nhập!");
            }
            throw new Exception("Lỗi khi gọi dữ liệu từ Server.");
        }

        public static async Task<HttpResponseMessage> PostAsJsonAsync(string endpoint, object obj)
        {
            AttachToken();
            return await _client.PostAsJsonAsync(endpoint, obj);
        }

        public static async Task<HttpResponseMessage> PutAsJsonAsync(string endpoint, object obj)
        {
            AttachToken();
            return await _client.PutAsJsonAsync(endpoint, obj);
        }

        public static async Task<HttpResponseMessage> DeleteAsync(string endpoint)
        {
            AttachToken();
            return await _client.DeleteAsync(endpoint);
        }
    }
}
