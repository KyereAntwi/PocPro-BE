using DevSync.PocPro.Accounts.Api.Features.Tenants.V1.GetSubAccountDetails;

namespace DevSync.PocPro.Accounts.Api.Features.Tenants.V1.CreateSubAccount;

public class CreateSubAccountEndpoint(
    Shared.Domain.CQRS.ICommandHandler<CreateSubAccountRequest, Guid> handler)
    : Endpoint<CreateSubAccountRequest, BaseResponse<Guid>>
{
    public override void Configure()
    {
        Post("/api/v1/accounts/tenants/sub-account");
        Description(x => x
            .WithName("CreateSubAccount")
            .Produces<BaseResponse<Guid>>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound));
    }

    public override async Task HandleAsync(CreateSubAccountRequest req, CancellationToken ct)
    {
        var result = await handler.HandleAsync(req, ct);

        if (result.IsFailed)
        {
            await SendAsync(new BaseResponse<Guid>("Failed to create sub-account", false)
                {
                    Errors = result.Errors.Select(e => e.Message).ToList()
                },
                (int)HttpStatusCode.BadRequest, ct);
        }
        
        await SendCreatedAtAsync<GetSubAccountDetailsEndpoint>(
            new { },
            new BaseResponse<Guid>("Sub-account created successfully.", true) { Data = result.Value },
            cancellation: ct);
    }
}

public class CreateSubAccountValidator : Validator<CreateSubAccountRequest>
{
    public CreateSubAccountValidator()
    {
        RuleFor(x => x.BusinessName)
            .NotEmpty().WithMessage("Business name is required.")
            .NotNull();
        
        RuleFor(x => x.SettlementBank)
            .NotEmpty().WithMessage("Settlement bank is required.")
            .NotNull();

        RuleFor(x => x.AccountNumber)
            .NotEmpty().WithMessage("Account number is required.")
            .NotNull();
    }
}