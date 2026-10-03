#nullable enable
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
        private static readonly HttpClient _client;

        static ApiClientService()
        {
            // Cấu hình bỏ qua xác thực chứng chỉ SSL tự ký ở môi trường Development
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };

            _client = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://localhost:7035/api/"),
                Timeout = TimeSpan.FromSeconds(30)
            };
        }

        public static async Task<bool> LoginAsync(string username, string password)
        {
            try
            {
                var loginObj = new { Username = username, Password = password };
                var response = await _client.PostAsJsonAsync("auth/login", loginObj);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(jsonString);

                    SessionManager.JwtToken = doc.RootElement.TryGetProperty("token", out var tokenProp) ? tokenProp.GetString() ?? string.Empty : string.Empty;
                    SessionManager.CurrentUsername = username;
                    SessionManager.CurrentRole = doc.RootElement.TryGetProperty("role", out var roleProp) ? roleProp.GetString() ?? string.Empty : string.Empty;

                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi kết nối API: {ex.Message}", ex);
            }
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
                throw new Exception("Phiên làm việc đã hết hạn hoặc chưa đăng nhập!");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("Tài khoản của bạn không có quyền truy cập chức năng này!");
            }

            var errDetail = await response.Content.ReadAsStringAsync();
            throw new Exception($"Lỗi từ Server ({response.StatusCode}): {errDetail}");
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