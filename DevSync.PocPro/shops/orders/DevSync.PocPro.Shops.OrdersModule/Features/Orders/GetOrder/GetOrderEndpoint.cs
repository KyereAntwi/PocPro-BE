namespace DevSync.PocPro.Shops.OrdersModule.Features.Orders.GetOrder;

public class GetOrderEndpoint : Endpoint<GetOrderRequest, BaseResponse<GetOrderResponse>>
{
    public override void Configure()
    {
        Get("/orders/{Id}");
    }

    public override Task HandleAsync(GetOrderRequest req, CancellationToken ct)
    {
        return base.HandleAsync(req, ct);
    }
}