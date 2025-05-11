using DevSync.PocPro.Accounts.Api.Features.Tenants.V1.GetTenantDetails;
using DevSync.PocPro.Shared.Domain.Utils;

namespace DevSync.PocPro.Accounts.Api.Features.Tenants.V1.CreateATenant;

public class CreateATenantEndpoint(
    IApplicationDbContext applicationDbContext, 
    ILogger<CreateATenantEndpoint> logger,
    IPublishEndpoint publishEndpoint,
    TenantDatabaseSettings tenantDatabaseSettings) 
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
        var existingUniqueIdentifier = await applicationDbContext.Tenants
            .Where(t => t.UniqueIdentifier.ToLower() == req.UniqueIdentifier.ToLower()).FirstOrDefaultAsync(ct);

        if (existingUniqueIdentifier != null)
        {
            await SendErrorsAsync((int)HttpStatusCode.BadRequest, ct);
            return;
        }
        
        var connectionString = $"Host={tenantDatabaseSettings.Server};Port={tenantDatabaseSettings.Port};Database={req.UniqueIdentifier};Username={tenantDatabaseSettings.Username};Password={tenantDatabaseSettings.Password};SSL Mode=Require;Trust Server Certificate=true;";
        _ = Enum.TryParse<SubscriptionType>(req.SubscriptionType, out var subscription);
        var tenant = Tenant.Create(req.UniqueIdentifier, connectionString, subscription);
        
        await applicationDbContext.Tenants.AddAsync(tenant, ct);
        await applicationDbContext.SaveChangesAsync(ct);
        
        // send an event for database creation and migration implementation
        var integrationEvent = new GenerateTenantDatabaseEvent
        {
            TenantId = tenant.Id.Value,
            DatabaseName = tenant.UniqueIdentifier
        };

        try
        {
            await publishEndpoint.Publish(integrationEvent, ct);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message, $"Error publishing integration event {nameof(GenerateTenantDatabaseEvent)}");
        }

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