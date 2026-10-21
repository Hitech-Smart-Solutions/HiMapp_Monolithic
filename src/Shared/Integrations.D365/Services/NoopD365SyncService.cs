using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Himapp.Integrations.D365.Services;

public sealed class NoopD365SyncService : ID365SyncService
{

    private readonly IConfiguration _config;
    // private readonly HimappDbContext _db;
    private readonly HttpClient _httpClient;
    private readonly ILogger<NoopD365SyncService> _logger;
    private static string _accessToken;
    private static DateTime _tokenExpiry;

    public NoopD365SyncService(IConfiguration config, IHttpClientFactory httpClientFactory, ILogger<NoopD365SyncService> logger)
        {
            _config = config;
            _httpClient = httpClientFactory.CreateClient();
            _logger = logger;
            _httpClient.Timeout = TimeSpan.FromSeconds(180);
        }

    public Task SyncItemsAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task SyncEquipmentAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task SyncActivitiesAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    #region Private Methods

    public async Task<string> GetDynamicsToken()
        {
            if (!string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _tokenExpiry)
                return _accessToken;

            var values = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" },
                { "client_id", _config["Dynamics:ClientId"] },
                { "client_secret", _config["Dynamics:ClientSecret"] },
                { "resource", _config["Dynamics:Resource"] }
            };

            var content = new FormUrlEncodedContent(values);

            var response = await _httpClient.PostAsync(
                $"https://login.microsoftonline.com/{_config["Dynamics:TenantId"]}/oauth2/token",
                content
            );

            if (!response.IsSuccessStatusCode)
                throw new Exception("Failed to generate Dynamics token");

            var json = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(json);

            _accessToken = result.access_token;
            _tokenExpiry = DateTime.UtcNow.AddSeconds((int)result.expires_in - 60);

            return _accessToken;
        }

        public async Task<List<T>> FetchFromDynamics<T>(string entityName, string filter)
        {
            var baseUrl = _config["Dynamics:BaseUrl"];
            var url = $"{baseUrl}/data/{entityName}?$filter={Uri.EscapeDataString(filter)}";

            try
            {
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Dynamics API Error. Entity: {Entity}, URL: {Url}, Status: {StatusCode}, Content: {Content}",
                        entityName, url, response.StatusCode, errorContent);

                    throw new Exception($"Dynamics API returned {response.StatusCode} for {entityName}.");
                }

                var json = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<ODataResponseDTO<T>>(json);

                return result?.Value ?? new List<T>();
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, "Network error reaching Dynamics at {Url}", url);
                throw;
            }
        }

        public async Task<bool> PatchToDynamics(string resourcePath, string token, object payload)
        {
            var url = $"{_config["Dynamics:BaseUrl"]}/data/{resourcePath}";

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Patch, url)
                {
                    Content = new StringContent(
                        JsonConvert.SerializeObject(payload),
                        Encoding.UTF8,
                        "application/json")
                };

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Dynamics PATCH failed for {Resource}. Status: {StatusCode}, Error: {Error}",
                        resourcePath, response.StatusCode, error);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while PATCHing Dynamics for {Resource}", resourcePath);
                return false;
            }
        }

        public async Task<bool> PostToDynamics(string resourcePath, string token, object payload)
        {
            try
            {
                var baseUrl = _config["Dynamics:BaseUrl"];
                if (string.IsNullOrEmpty(baseUrl))
                {
                    throw new Exception("Dynamics BaseUrl is not configured.");
                }

                var url = $"{baseUrl}/data/{resourcePath}";
                var jsonPayload = JsonConvert.SerializeObject(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                // Set the bearer token for this specific request
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"Successfully posted to Dynamics: {resourcePath}");
                    return true;
                }
                else
                {
                    var errorDetails = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Dynamics API returned an error. Status: {response.StatusCode}, Content: {errorDetails}");
                    return false;
                }
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, $"Network error occurred while posting to Dynamics resource: {resourcePath}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unexpected error occurred in PostToDynamics for resource: {resourcePath}");
                return false;
            }
        }

        public DateTime? NormalizeDynamicsDate(DateTime? date = null)
        {
            DateTime activeDate = date ?? DateTime.UtcNow;

            if (activeDate.Year <= 1900)
            {
                return null;
            }

            return DateTime.SpecifyKind(activeDate, DateTimeKind.Utc);
        }

    #endregion
}
