namespace DevSync.PocPro.Accounts.Api.Features.Tenants.V1.CreateATenant;

public class CreateATenantEndpoint(
    Shared.Domain.CQRS.ICommandHandler<CreateATenantRequest, CreateATenantResponse> handler,
    ApiAuthentication apiAuthentication,
    IHttpContextAccessor httpContextAccessor) 
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
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreateATenantRequest req, CancellationToken ct)
    {
        string? appKey = httpContextAccessor.HttpContext?.Request.Headers["Api-Key"].FirstOrDefault();

        if (
            string.IsNullOrWhiteSpace(appKey) || 
            !string.Equals(appKey, apiAuthentication.OnBoardingAppKey)) 
        { 
            await SendUnauthorizedAsync(ct);
            return;
        }

        var result = await handler.HandleAsync(req, ct);
        
        if (result.IsFailed)
        {
            await SendAsync(new BaseResponse<CreateATenantResponse>("Failed to create tenant", false)
            {
                Errors = result.Errors.Select(e => e.Message).ToList()
            }, StatusCodes.Status400BadRequest, ct);
        }

        await SendCreatedAtAsync<GetTenantDetailsEndpoint>(new
            {
                Id = result.Value.Id
            }, 
            new BaseResponse<CreateATenantResponse>("Tenant created successfully", true)
            {
                Data = new CreateATenantResponse(result.Value.Id)
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