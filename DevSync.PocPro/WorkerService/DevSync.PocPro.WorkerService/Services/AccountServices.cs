using System.Net.Http.Json;
using DevSync.PocPro.WorkerService.Utils;
using Polly;
using Polly.Retry;

namespace DevSync.PocPro.WorkerService.Services;

public class AccountServices(ExternalUrls externalUrls, HttpClient httpClient, ILogger<PosServices> logger) 
    : IAccountServices
{
    private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy = Policy
        .Handle<HttpRequestException>()
        .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    
    public async Task<IEnumerable<string>> GetAccountIdentifiersAsync(CancellationToken cancellationToken = default)
    {
        var url = $"{externalUrls.AccountApi}/api/v1/tenants/identifiers/for-worker-service";
        
        var response = await _retryPolicy.ExecuteAsync(() =>
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            AddIdentifierHeader(request);
            return httpClient.SendAsync(request, cancellationToken);
        });
        
        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadFromJsonAsync<IEnumerable<string>>(cancellationToken: cancellationToken);
            return data?.ToList() ?? [];
        }
        
        logger.LogError("Failed to fetch Identifiers at {Time}. Status code: {StatusCode}", DateTime.UtcNow, response.StatusCode);
        throw new HttpRequestException($"Failed to fetch identifiers from {url}. Status code: {response.StatusCode}");
    }
    
    private void AddIdentifierHeader(HttpRequestMessage request)
    {
        request.Headers.Add("X-Api-Token", externalUrls.ApiKey);
    }
}