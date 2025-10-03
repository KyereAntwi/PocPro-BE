namespace DevSync.PocPro.Accounts.Api.Features.ApplicationUsers.V1.AddApplicationUser;

public class AddApplicationUserEndpoint(
    Shared.Domain.CQRS.ICommandHandler<AddApplicationUserRequest, AddApplicationUserResponse> handler) 
    : Endpoint<AddApplicationUserRequest, BaseResponse<AddApplicationUserResponse>>
{
    public override void Configure()
    {
        Post("/api/v1/accounts/users");
        Description(x => x
            .WithName("AddUser")
            .Produces<BaseResponse<AddApplicationUserResponse>>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
        );
    }

    public override async Task HandleAsync(AddApplicationUserRequest req, CancellationToken ct)
    {
        var result = await handler.HandleAsync(req, ct);

        if (result.IsFailed)
        {
            await SendAsync(new BaseResponse<AddApplicationUserResponse>("Failed to add application user", false)
            {
                Errors = result.Errors.Select(e => e.Message).ToList()
            }, StatusCodes.Status400BadRequest, ct);
        }

        await SendCreatedAtAsync<GetApplicationUserDetailsEndpoint>(new
            {
                result.Value.Id,
            },
            new BaseResponse<AddApplicationUserResponse>("Application created successfully", true)
            {
                Data = result.Value
            }, cancellation: ct);
    }
}

public class AddApplicationUserRequestValidator : Validator<AddApplicationUserRequest>
{
    public AddApplicationUserRequestValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().WithMessage("First name cannot be empty.").NotNull();
        RuleFor(x => x.LastName).NotEmpty().WithMessage("Last name cannot be empty.").NotNull();
        
        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.Email)).WithMessage("Email should be a valid email address").NotNull();
        
        RuleFor(x => x.Password)
            .NotEmpty()
            .When(x => !x.TenantAccount).WithMessage("Password cannot be empty.").NotNull();
        
        RuleFor(x => x.Username).NotEmpty().WithMessage("Username cannot be empty.").NotNull();
        RuleFor(x => x.TenantId).NotEmpty().WithMessage("Tenant id cannot be empty.").NotNull();
        
        RuleFor(x => x.PermissionTypes)
            .Must(list => list == null || list.All(pt => Enum.TryParse<PermissionType>(pt, out _)))
            .When(x => x.PermissionTypes != null && x.PermissionTypes.Any())
            .WithMessage("All permission types must be valid PermissionType enum values.");
    }
}