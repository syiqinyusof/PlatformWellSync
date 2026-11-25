using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using PlatformWellSync.Dtos;

namespace PlatformWellSync.Services;

public class AemApiClient
{
    private readonly HttpClient _http;

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public AemApiClient(string baseUrl)
    {
        _http = new HttpClient { BaseAddress = new Uri(baseUrl) };    
    }

    public async Task LoginAsync(string username, string password)
    {
        var response = await _http.PostAsJsonAsync("/api/Account/Login", new { username, password });
        response.EnsureSuccessStatusCode();

        var token = (await response.Content.ReadAsStringAsync()).Trim().Trim('"');
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<List<PlatformDto>> GetPlatformWellsAsync(string endpoint)
    {
        // download the JSON and fills DTO (delivery boxes) with data from AEM API
        var data = await _http.GetFromJsonAsync<List<PlatformDto>>(endpoint, JsonOptions);
        return data ?? new List<PlatformDto>();
    }
}