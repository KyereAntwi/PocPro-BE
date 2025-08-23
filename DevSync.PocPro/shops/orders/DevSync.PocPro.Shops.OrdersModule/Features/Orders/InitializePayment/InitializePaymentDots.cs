namespace DevSync.PocPro.Shops.OrdersModule.Features.Orders.InitializePayment;

public record InitializePaymentRequest(
    decimal Amount,
    string TenantIdentifier,
    string CustomerEmail);
    
public record InitializePaymentResponse(
    string AccessCode,
    string AuthorizationUrl,
    string Reference);