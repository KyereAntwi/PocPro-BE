using DevSync.PocPro.Accounts.Api.Features.Tenants.V1.GetTenantDetails;

namespace DevSync.PocPro.Accounts.Api.Features.Tenants.V1.CreateATenant;

public class CreateATenantEndpoint(
    IApplicationDbContext applicationDbContext, ILogger<CreateATenantEndpoint> logger) 
    : Endpoint<CreateATenantRequest, BaseResponse<CreateATenantResponse>>
{
    public override void Configure()
    {
        Post("/api/v1/accounts/tenants");
        Description(x => x
            .WithName("CreateTenant")
            .Produces<BaseResponse<CreateATenantResponse>>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
        );
    }

    public override async Task HandleAsync(CreateATenantRequest req, CancellationToken ct)
    {
        const string connectionString = $"";
        _ = Enum.TryParse<SubscriptionType>(req.SubscriptionType, out var subscription);
        var tenant = Tenant.Create(req.UniqueIdentifier, connectionString, subscription);
        
        await applicationDbContext.Tenants.AddAsync(tenant, ct);
        await applicationDbContext.SaveChangesAsync(ct);
        
        // TODO - send for database implementation

        await SendCreatedAtAsync<GetTenantDetailsEndpoint>(new { tenant.Id }, 
            new BaseResponse<CreateATenantResponse>("Tenant created successfully", true)
            {
                Data = new CreateATenantResponse(tenant.Id.Value)
            }, cancellation: ct);
    }
}

public class CreateATenantRequestValidator : Validator<CreateATenantRequest>
{
    public CreateATenantRequestValidator()
    {
        RuleFor(x => x.UniqueIdentifier)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .NotNull();
        
        RuleFor(x => x.SubscriptionType)
            .NotEmpty().WithMessage("Subscription cannot be empty")
            .IsEnumName(typeof(SubscriptionType)).WithMessage("Subscription should contain valid subscription type")
            .NotNull();
    }
}