using System.Text;
using System.Text.Json;
using DevSync.PocPro.Identity.Dtos;
using Polly;
using Polly.Retry;

namespace DevSync.PocPro.Identity.Services;

public class AccountApiServices : IAccountsApiServices
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy = Policy
        .Handle<HttpRequestException>()
        .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

    public AccountApiServices(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<int> AddUserToAccountsAsync(AddUserToAccountRequest payload)
    {
        try
        {
            var url = $"{_configuration.GetConnectionString("AccountsApiBaseUrl")}/api/v1/accounts";
            var actualLoad = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            
            var response = await _retryPolicy.ExecuteAsync(() => {
                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Content = actualLoad;
                return _httpClient.SendAsync(request);
            });

            return (int)response.StatusCode;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}