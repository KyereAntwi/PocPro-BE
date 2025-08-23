using System.Globalization;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using DevSync.PocPro.Shops.Shard.Grpc;
using Polly;
using Polly.Retry;

namespace DevSync.PocPro.Shops.OrdersModule.Services;

public class PaymentService(
    Shard.Grpc.PaymentService.PaymentServiceClient paymentServiceClient, 
    ILogger<PaymentService> logger,
    ApiAuthentication apiAuthentication,
    TenantServiceSettings tenantServiceSettings)
    : IPaymentServices
{
    private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy = Policy
        .Handle<HttpRequestException>()
        .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    
    public async Task<bool> VerifyAsync(string paymentRef, CancellationToken token = default)
    {
        var request = new VerifyPaymentRequest
        {
            PaymentReference = paymentRef,
            App = "Shop Service"
        };

        try
        {
            var metadata = new Metadata
            {
                { "x-api-key", apiAuthentication.ApiKey }
            };
            
            var response = await paymentServiceClient.VerifyPaymentAsync(request, metadata, cancellationToken: token);
            return response.IsSuccess;
        }
        catch (Exception ex)
        {
            logger.LogError("Error verifying payment. Error message: {Message}", ex.Message);
            return false;
        }
    }

    public async Task<(string, string, string)> InitializeAsync(decimal amount, string customerEmail, string subAccount, CancellationToken token = default)
    {
        var request = new InitializePaymentRequest
        {
            Email = customerEmail,
            Amount = (amount * 100).ToString(CultureInfo.InvariantCulture),
            SubAccount = subAccount
        };

        try
        {
            var metadata = new Metadata
            {
                { "x-api-key", apiAuthentication.ApiKey }
            };

            var response = await paymentServiceClient.InitializePaymentAsync(request, metadata, cancellationToken: token);
            return response is not { IsSuccess: true } ? 
                ("", "", "") : 
                (response.AccessCode, response.AuthorizationUrl, response.Reference);
        }
        catch (Exception e)
        {
            logger.LogError("Error verifying payment. Error message: {Message}", e.Message);
            return ("", "", "");
        }
    }

    public async Task<bool> VerifyHttpAsync(string paymentRef, CancellationToken token = default)
    {
        var load = new
        {
            PaymentReference = paymentRef,
            App = "Shop Service"
        };
        
        var json = JsonSerializer.Serialize(load);

        try
        {
            var response = await _retryPolicy.ExecuteAsync(() => {
                var payload = new HttpRequestMessage(HttpMethod.Post, $"{tenantServiceSettings.PaymentServiceUrl}/payments/verify")
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
                
                AddAuthorizationHeader(payload);
                
                return new HttpClient().SendAsync(payload, token);
            });
            
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("Failed to initialize payment: {StatusCode} - {ReasonPhrase}", 
                    response.StatusCode, response.ReasonPhrase);
                return false;
            }
            
            var result = await response.Content.ReadFromJsonAsync<InitializePaymentResponse>(token);
             
            return result is not null;
        }
        catch (Exception e)
        {
            logger.LogError("Error occurred during payment initialization at {TimStamp}. Error = : {Message}", DateTime.UtcNow, e.Message);
            return false;
        }
    }

    public async Task<(string, string, string)> InitializeHttpAsync(decimal amount, string customerEmail, string subAccount, CancellationToken token = default)
    {
        var load = new
        {
            email = customerEmail,
            amount = (amount * 100).ToString(CultureInfo.InvariantCulture),
            subaccount = subAccount,
        };
            
        var json = JsonSerializer.Serialize(load);
            
        try
        {
            var response = await _retryPolicy.ExecuteAsync(() => {
                var payload = new HttpRequestMessage(HttpMethod.Post, $"{tenantServiceSettings.PaymentServiceUrl}/payments/initialize")
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
                
                AddAuthorizationHeader(payload);
                
                return new HttpClient().SendAsync(payload, token);
            });

            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("Failed to initialize payment: {StatusCode} - {ReasonPhrase}", 
                    response.StatusCode, response.ReasonPhrase);
                return ("", "", "");
            }

            var result = await response.Content.ReadFromJsonAsync<InitializePaymentResponse>(token);
             
            return result is null ? 
                ("", "", "") : 
                (result.AccessCode, result.AuthorizationUrl, result.Reference);
        }
        catch (Exception e)
        {
            logger.LogError("Error occurred during payment initialization at {TimStamp}. Error = : {Message}", DateTime.UtcNow, e.Message);
            return ("", "", "");
        }
    }

    private void AddAuthorizationHeader(HttpRequestMessage request)
    {
        request.Headers.Add("x-api-key", apiAuthentication.ApiKey);
    }
}