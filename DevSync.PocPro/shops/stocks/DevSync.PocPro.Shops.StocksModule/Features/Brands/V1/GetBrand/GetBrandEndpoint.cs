namespace DevSync.PocPro.Shops.StocksModule.Features.Brands.V1.GetBrand;

public class GetBrandEndpoint(IShopDbContext shopDbContext) 
    : Endpoint<GetBrandRequest, BaseResponse<BrandResponse>>
{
    public override void Configure()
    {
        Get("/api/v1/brands/{Id}");
        Description(x => x
            .WithName("GetBrand")
            .Produces<BaseResponse<BrandResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError));
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetBrandRequest req, CancellationToken ct)
    {
        var brand = await shopDbContext
            .Brands
            .FindAsync(BrandId.Of(req.Id), ct);

        if (brand is null)
        {
            await SendAsync(new BaseResponse<BrandResponse>("Operation failed" , false)
            {
                Errors = new List<string> { $"Not found" }
            }, (int)HttpStatusCode.NotFound , ct);
            return;
        }

        await SendOkAsync(new BaseResponse<BrandResponse>("Brand retrieved successfully", true)
        {
            Data = new BrandResponse(brand.Id.Value, brand.Title, brand.Description, brand.ImageUrl)
        }, ct);
    }
}