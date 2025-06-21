using DevSync.PocPro.Shared.Domain.Enums;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using DevSync.PocPro.Shared.Domain.Dtos;
using DevSync.PocPro.Shops.Shared.Interfaces;
using DevSync.PocPro.Shops.Shared.Utils;
using Microsoft.AspNetCore.Http;
using Polly;
using Polly.Retry;

namespace DevSync.PocPro.Shops.Shared.Services;

public class TenantServices(
    HttpClient httpClient, ILogger<TenantServices> logger, IHttpContextAccessor httpContextAccessor, TenantServiceSettings tenantServiceSettings) 
    : ITenantServices
{
    private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy = Policy
        .Handle<HttpRequestException>()
        .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

    public async Task<Tenant?> GetTenantByUserIdAsync(string userId)
    {
        try
        {
            var url = $"{tenantServiceSettings.BaseUrl}/api/v1/accounts/tenants/{userId}";
            
            var response = await _retryPolicy.ExecuteAsync(() => {
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                AddAuthorizationHeader(request);
                return httpClient.SendAsync(request);
            });
            
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("Tenant not found for user {UserId}: {StatusCode}", userId, response.StatusCode);
                throw new Exception("Tenant or User not found");
            }
            var data = await response.Content.ReadFromJsonAsync<BaseResponse<TenantDto>>();
            var tenantDto = data!.Data;
            return tenantDto != null ? new Tenant(tenantDto.ConnectionString, userId, tenantDto.SubscriptionType) : null;
        }
        catch (Exception e)
        {
            logger.LogError("Error fetching tenant details for user {UserId}: {Message}", userId, e.Message);
            throw new Exception("Error fetching tenant details", e);
        }
    }

    public async Task<Tenant?> GetTenantByIdentifierAsync(string identifier)
    {
        try
        {
            var url = $"{tenantServiceSettings.BaseUrl}/api/v1/accounts/tenants/byidentifier/{identifier}";

            var response = await _retryPolicy.ExecuteAsync(() =>
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                AddAuthorizationHeader(request);
                return httpClient.SendAsync(request);
            });
            
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("Tenant not found for user with Identifier {UserId}: {StatusCode}", identifier, response.StatusCode);
                throw new Exception("Tenant or User not found");
            }
            var data = await response.Content.ReadFromJsonAsync<BaseResponse<TenantDto>>();
            var tenantDto = data!.Data;
            return tenantDto != null ? new Tenant(tenantDto.ConnectionString, identifier, tenantDto.SubscriptionType) : null;
        }
        catch (Exception e)
        {
            logger.LogError("Error fetching tenant details for user with {UserId}: {Message}", identifier, e.Message);
            throw new Exception("Error fetching tenant details", e);
        }
    }

    public async Task<IEnumerable<Tenant>> GetAllTenantsAsync()
    {
        try
        {
            // var response = await _retryPolicy.ExecuteAsync(() =>
            //     httpClient.GetAsync($"{Baseurl}/api/v1/accounts/tenants")
            // );
            
            // var request = new HttpRequestMessage(HttpMethod.Get, $"{Baseurl}/api/v1/accounts/tenants");
            // AddAuthorizationHeader(request);
            // var response = await _retryPolicy.ExecuteAsync(() => httpClient.SendAsync(request));
            
            var url = $"{tenantServiceSettings.BaseUrl}/api/v1/accounts/tenants";
            
            var response = await _retryPolicy.ExecuteAsync(() => {
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                AddAuthorizationHeader(request);
                return httpClient.SendAsync(request);
            });
            
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("Error fetching tenants: {StatusCode}", response.StatusCode);
                throw new Exception("Error fetching tenants");
            }
            var data = await response.Content.ReadFromJsonAsync<BaseResponse<IEnumerable<TenantDto>>>();
            var tenants = data!.Data ?? [];
            return tenants?.Select(t => new Tenant(t.ConnectionString, t.UniqueIdentifier, t.SubscriptionType)) ?? [];
        }
        catch (Exception e)
        {
            logger.LogError("Error fetching tenants: {Message}", e.Message);
            throw new Exception("Error fetching tenants", e);
        }
    }

    public async Task<bool> UserHasRequiredPermissionAsync(PermissionType permissionType, string userId)
    {
        try
        {
            // var response = await _retryPolicy.ExecuteAsync(() =>
            //     httpClient.GetAsync($"{Baseurl}/api/v1/accounts/tenants/user/{userId}/permissions/{permissionType}")
            // );
            
            // var request = new HttpRequestMessage(HttpMethod.Get, $"{Baseurl}/api/v1/accounts/users/{userId}/permissions/{permissionType}");
            // AddAuthorizationHeader(request);
            // var response = await _retryPolicy.ExecuteAsync(() => httpClient.SendAsync(request));

            var url = $"{tenantServiceSettings.BaseUrl}/api/v1/accounts/users/{userId}/permissions/{permissionType}";
            
            var response = await _retryPolicy.ExecuteAsync(() => {
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                AddAuthorizationHeader(request);
                return httpClient.SendAsync(request);
            });
            
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("Error fetching permission for user {UserId}: {StatusCode}", userId, response.StatusCode);
                throw new Exception("Error fetching user permission");
            }
            var result = await response.Content.ReadFromJsonAsync<BaseResponse<HasPermissionDto>>();
            return result!.Data!.HasPermission;
        }
        catch (Exception e)
        {
            logger.LogError("Error fetching permission: {Message}", e.Message);
            throw new Exception("Error fetching user permission", e);
        }
    }
    
    private void AddAuthorizationHeader(HttpRequestMessage request)
    {
        var accessToken = httpContextAccessor.HttpContext?.Request.Headers.Authorization.FirstOrDefault();
        if (string.IsNullOrEmpty(accessToken)) return;
        
        if (!accessToken.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            accessToken = $"Bearer {accessToken}";
        request.Headers.Add("Authorization", accessToken);
    }
}

// DTO for deserialization
public class TenantDto
{
    public string ConnectionString { get; set; } = string.Empty;
    public string UniqueIdentifier { get; set; } = string.Empty;
    public string SubscriptionType { get; set; } = string.Empty;
}

public class HasPermissionDto
{
    public bool HasPermission { get; set; }
}

