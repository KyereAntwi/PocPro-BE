using System.Net.Http.Json;
using DevSync.PocPro.Shops.Shared.Dtos;
using DevSync.PocPro.Shops.Shared.Utils;
using DevSync.PocPro.WorkerService.Interfaces;
using DevSync.PocPro.WorkerService.Utils;
using Polly;
using Polly.Retry;

namespace DevSync.PocPro.WorkerService.Services;

public class ProductServices(ExternalUrls externalUrls, HttpClient httpClient, ILogger<PosServices> logger) : IProductServices
{
    private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy = Policy
        .Handle<HttpRequestException>()
        .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    
    public async Task<IReadOnlyList<ProductDto>> GetProductsLowInStockAsync(List<Guid> posIds, string identifier, CancellationToken cancellationToken = default)
    {
        var posParams = string.Join(";", posIds);
        var url = $"{externalUrls.ShopApi}/api/v1/products/for-service-worker/get-products-low-in-stock?Pos={posParams}";

        var response = await _retryPolicy.ExecuteAsync(() =>
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            AddIdentifierHeader(request, identifier);
            return httpClient.SendAsync(request, cancellationToken);
        });
        
        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("Failed to fetch products low in stock at {Time}. Status code: {StatusCode}", DateTime.UtcNow, response.StatusCode);
            throw new HttpRequestException($"Failed to fetch products low in stock from {url}. Status code: {response.StatusCode}");
        }
        
        var data = await response.Content.ReadFromJsonAsync<IEnumerable<ProductDto>>(cancellationToken: cancellationToken);
        return data?.ToList() ?? [];
    }

    public async Task<IReadOnlyList<ProductDto>> GetProductsExpiringSoonAsync(List<Guid> posIds, string identifier, CancellationToken cancellationToken = default)
    {
        var posParams = string.Join(";", posIds);
        var url = $"{externalUrls.ShopApi}/api/v1/products/for-service-worker/get-products-expiring-soon?Pos={posParams}";

        var response = await _retryPolicy.ExecuteAsync(() =>
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            AddIdentifierHeader(request, identifier);
            return httpClient.SendAsync(request, cancellationToken);
        });
        
        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("Failed to fetch products expiring soon at {Time}. Status code: {StatusCode}", DateTime.UtcNow, response.StatusCode);
            throw new HttpRequestException($"Failed to fetch products expiring soon from {url}. Status code: {response.StatusCode}");
        }
        
        var data = await response.Content.ReadFromJsonAsync<IEnumerable<ProductDto>>(cancellationToken: cancellationToken);
        return data?.ToList() ?? [];
    }
    
    private void AddIdentifierHeader(HttpRequestMessage request, string identifier)
    {
        request.Headers.Add("X-Tenant-Identifier", identifier);
        request.Headers.Add("X-Api-Key", externalUrls.ApiKey);
    }
}