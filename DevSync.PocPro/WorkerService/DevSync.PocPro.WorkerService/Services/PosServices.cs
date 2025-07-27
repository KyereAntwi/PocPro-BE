using System.Net.Http.Json;
using DevSync.PocPro.WorkerService.Utils;
using Polly;
using Polly.Retry;

namespace DevSync.PocPro.WorkerService.Services;

public class PosServices(ExternalUrls externalUrls, HttpClient httpClient, ILogger<PosServices> logger) : IPosServices
{
    private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy = Policy
        .Handle<HttpRequestException>()
        .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    
    public async Task<IReadOnlyList<Guid>> GetAllPosIdsAsync(string identifier, CancellationToken cancellationToken = default)
    {
        var url = $"{externalUrls.ShopApi}/api/v1/pointofsales/get-all/for-service-worker";

        var response = await _retryPolicy.ExecuteAsync(() =>
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            AddIdentifierHeader(request, identifier);
            return httpClient.SendAsync(request, cancellationToken);
        });
        
        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("Failed to fetch POS IDs at {Time}. Status code: {StatusCode}", DateTime.UtcNow, response.StatusCode);
            throw new HttpRequestException($"Failed to fetch POS IDs from {url}. Status code: {response.StatusCode}");
        }
        
        var data = await response.Content.ReadFromJsonAsync<IEnumerable<Guid>>(cancellationToken: cancellationToken);
        return data?.ToList() ?? [];
    }

    private void AddIdentifierHeader(HttpRequestMessage request, string identifier)
    {
        request.Headers.Add("X-Tenant-Identifier", identifier);
        request.Headers.Add("X-Api-Token", externalUrls.ApiKey);
    }
}