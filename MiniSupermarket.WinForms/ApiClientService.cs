#nullable enable
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace MiniSupermarket.WinForms
{
    public static class ApiClientService
    {
        private static readonly string[] CandidateBaseUrls =
        {
            "https://localhost:7046/api/",
            "https://localhost:7035/api/",
            "http://localhost:5081/api/"
        };

        private static readonly HttpClient[] ClientPool;
        private static HttpClient _activeClient;

        static ApiClientService()
        {
            var handlerFactory = new Func<HttpClientHandler>(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            });

            ClientPool = CandidateBaseUrls
                .Select(url => new HttpClient(handlerFactory())
                {
                    BaseAddress = new Uri(url),
                    Timeout = TimeSpan.FromSeconds(30)
                })
                .ToArray();

            _activeClient = ClientPool[0];
        }

        public static async Task EnsureActiveBaseAddressAsync()
        {
            if (_activeClient is not null)
            {
                try
                {
                    using var probe = await _activeClient.GetAsync("customers");
                    if (probe.StatusCode == System.Net.HttpStatusCode.NotFound || probe.StatusCode == System.Net.HttpStatusCode.Unauthorized || probe.StatusCode == System.Net.HttpStatusCode.Forbidden || probe.IsSuccessStatusCode)
                    {
                        return;
                    }
                }
                catch (HttpRequestException)
                {
                }
                catch (TaskCanceledException)
                {
                }
            }

            foreach (var client in ClientPool)
            {
                try
                {
                    using var probe = await client.GetAsync("customers");
                    if (probe.StatusCode == System.Net.HttpStatusCode.NotFound || probe.StatusCode == System.Net.HttpStatusCode.Unauthorized || probe.StatusCode == System.Net.HttpStatusCode.Forbidden || probe.IsSuccessStatusCode)
                    {
                        _activeClient = client;
                        return;
                    }
                }
                catch (HttpRequestException)
                {
                }
                catch (TaskCanceledException)
                {
                }
            }

            _activeClient = ClientPool[0];
        }

        public static async Task<bool> LoginAsync(string username, string password)
        {
            try
            {
                await EnsureActiveBaseAddressAsync();
                var activeClient = _activeClient ?? ClientPool[0];
                var loginObj = new { Username = username, Password = password };
                var response = await activeClient.PostAsJsonAsync("auth/login", loginObj);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(jsonString);

                    SessionManager.JwtToken = doc.RootElement.TryGetProperty("token", out var tokenProp) ? tokenProp.GetString() ?? string.Empty : string.Empty;
                    SessionManager.CurrentUsername = username;
                    SessionManager.CurrentRole = doc.RootElement.TryGetProperty("role", out var roleProp) ? roleProp.GetString() ?? string.Empty : string.Empty;
                    SessionManager.CurrentDisplayName = string.IsNullOrWhiteSpace(SessionManager.CurrentUsername) ? username : SessionManager.CurrentUsername;

                    return true;
                }
            }
            catch (Exception)
            {
                // Dùng dữ liệu mẫu khi API chưa chạy hoặc chưa có DB.
            }

            var demoAccount = RolePermissionCatalog.FindDemoAccount(username, password);
            if (demoAccount != null)
            {
                SessionManager.JwtToken = $"demo:{demoAccount.Username}";
                SessionManager.CurrentUsername = demoAccount.Username;
                SessionManager.CurrentDisplayName = demoAccount.FullName;
                SessionManager.CurrentRole = demoAccount.Role;
                return true;
            }

            return false;
        }

        private static HttpClient GetActiveClient()
        {
            return _activeClient ??= ClientPool[0];
        }

        private static void AttachToken()
        {
            var activeClient = GetActiveClient();

            if (!string.IsNullOrEmpty(SessionManager.JwtToken))
            {
                activeClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", SessionManager.JwtToken);
            }
            else
            {
                activeClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        public static async Task<T?> GetAsync<T>(string endpoint)
        {
            await EnsureActiveBaseAddressAsync();
            var activeClient = GetActiveClient();
            AttachToken();
            var response = await activeClient.GetAsync(endpoint);

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
            await EnsureActiveBaseAddressAsync();
            var activeClient = GetActiveClient();
            AttachToken();
            return await activeClient.PostAsJsonAsync(endpoint, obj);
        }

        public static async Task<HttpResponseMessage> PutAsJsonAsync(string endpoint, object obj)
        {
            await EnsureActiveBaseAddressAsync();
            var activeClient = GetActiveClient();
            AttachToken();
            return await activeClient.PutAsJsonAsync(endpoint, obj);
        }

        public static async Task<HttpResponseMessage> DeleteAsync(string endpoint)
        {
            await EnsureActiveBaseAddressAsync();
            var activeClient = GetActiveClient();
            AttachToken();
            return await activeClient.DeleteAsync(endpoint);
        }
    }
}