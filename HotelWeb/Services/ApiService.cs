using System.Net.Http.Json;

namespace HotelWeb.Services;

public class ApiService
{
    private readonly HttpClient _http;
    public ApiService(HttpClient http) => _http = http;

    public Task<T?> GetAsync<T>(string url) => _http.GetFromJsonAsync<T>(url);
    public Task<HttpResponseMessage> PostAsync<T>(string url, T data) => _http.PostAsJsonAsync(url, data);
    public Task<HttpResponseMessage> PutAsync<T>(string url, T data) => _http.PutAsJsonAsync(url, data);
    public Task<HttpResponseMessage> PutNoBodyAsync(string url) => _http.PutAsync(url, null);
    public Task<HttpResponseMessage> DeleteAsync(string url) => _http.DeleteAsync(url);
}