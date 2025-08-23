namespace DevSync.PocPro.Shops.OrdersModule.Features.Orders.InitializePayment;

public class InitializePaymentEndpoint(
    ITenantServices tenantServices,
    IPaymentServices paymentServices) 
    : Endpoint<InitializePaymentRequest, BaseResponse<InitializePaymentResponse>>
{
    public override void Configure()
    {
        Post("/api/v1/orders/initialize-payment");
        AllowAnonymous();
    }

    public override async Task HandleAsync(InitializePaymentRequest req, CancellationToken ct)
    {
        var tenant = await tenantServices.GetTenantByIdentifierAsync(req.TenantIdentifier);
        
        if (tenant == null)
        {
            await SendAsync(
                new BaseResponse<InitializePaymentResponse>("Tenant not found", false)
                {
                    Errors = ["The specified tenant does not exist."]
                },
                StatusCodes.Status404NotFound, ct);
            return;
        }
        
        var (accessCode, authorizationUrl, reference) = await paymentServices.InitializeHttpAsync(
            req.Amount, req.CustomerEmail, tenant.SubAccount!, ct);

        if (string.IsNullOrWhiteSpace(accessCode) || string.IsNullOrWhiteSpace(authorizationUrl) ||
            string.IsNullOrWhiteSpace(reference))
        {
            await SendErrorsAsync(cancellation: ct);
            return;
        }
        
        await SendAsync(
            new BaseResponse<InitializePaymentResponse>(
                "Payment initialized successfully", true)
            {
                Data = new InitializePaymentResponse(accessCode, authorizationUrl, reference)
            },
            StatusCodes.Status200OK, ct);
    }
}