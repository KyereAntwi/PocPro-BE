using Polly;
using Polly.Retry;

namespace DevSync.PocPro.Accounts.Api.Services;

public class KeycloakServices : IKeycloakServices
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<KeycloakServices> _logger;
    private readonly KeycloakSettings _keycloakSettings;
    private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

    public KeycloakServices(HttpClient httpClient, ILogger<KeycloakServices> logger, KeycloakSettings keycloakSettings)
    {
        _httpClient = httpClient;
        _logger = logger;
        _keycloakSettings = keycloakSettings;

        _retryPolicy = Policy
            .Handle<HttpRequestException>()
            .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }
    
    public async Task RegisterUserLoginAsync(string username, string email)
    {
        var userPayload = new
        {
            username = username,
            email = email,
            emailVerified = true,
            credentials = new List<CredentialRepresentation>
            {
                new()
                {
                    Type = _keycloakSettings.CredentialType,
                    UserLabel = "My Password",
                    SecretData = "",
                    CredentialData = "",
                    Temporary = false
                }
            }
        };

        var response = await _retryPolicy
            .ExecuteAsync(() =>
                _httpClient.PostAsJsonAsync(_keycloakSettings.Url, userPayload));

        if (!response.IsSuccessStatusCode)
        {
            _logger
                .LogError("Failed to create user with email {Email} and username {Username} in Keycloak.", userPayload.email, userPayload.username);
        }
    }
}

public record CredentialRepresentation
{
    public string Type { get; set; } = string.Empty;
    public string UserLabel { get; set; } = string.Empty;
    public string SecretData { get; set; } = string.Empty;
    public string CredentialData { get; set; } = string.Empty;
    public bool Temporary { get; set; }
}