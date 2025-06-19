using System.Text;
using System.Text.Json;
using Polly;
using Polly.Retry;

namespace DevSync.PocPro.Accounts.Api.Services;

public class IdentityServices : IIdentityServices
{
    private readonly HttpClient _httpClient;
    private readonly KeycloakSettings _keycloakSettings;
    private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

    public IdentityServices(
        HttpClient httpClient,
        KeycloakSettings keycloakSettings)
    {
        _httpClient = httpClient;
        _keycloakSettings = keycloakSettings;

        _retryPolicy = Policy
            .Handle<HttpRequestException>()
            .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }
    
    public async Task<string> RegisterUserLoginAsync(string username, string email, string password, string accessToken)
    {
        var url = $"{_keycloakSettings.Url}/api/v1/users/register";
        
        var payload = new UserLoginRequest(username, email, password);
        var json = JsonSerializer.Serialize(payload);
            
        var result = await _retryPolicy.ExecuteAsync(() =>
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            AddAuthorizationHeader(request, accessToken);
            return _httpClient.SendAsync(request);
        });
            
        return await result.Content.ReadAsStringAsync();
    }
    
    private void AddAuthorizationHeader(HttpRequestMessage request, string accessToken)
    {
        if (string.IsNullOrEmpty(accessToken)) return;
        
        if (!accessToken.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            accessToken = $"Bearer {accessToken}";
        request.Headers.Add("Authorization", accessToken);
    }
}

public record UserLoginRequest(string Username, string Email, string Password);