namespace DevSync.PocPro.Accounts.Api.Features.ApplicationUsers.V1.UpdateUserProfile;

public class UpdateUserProfileEndpoint(IApplicationDbContext applicationDbContext)
    : Endpoint<UpdateUserProfileRequest>
{
    public override void Configure()
    {
        Put("/accounts/users/{UserId}");
        Description(x => x
            .WithName("UpdateUserProfile")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
        );
    }

    public override async Task HandleAsync(UpdateUserProfileRequest req, CancellationToken ct)
    {
        var user = await applicationDbContext.ApplicationUsers
            .FirstOrDefaultAsync(x => x.UserId == req.UserId, ct);

        if (user == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var photoUrl = user.PhotoUrl;
        if (req.PhotoUrl != req.PhotoUrl)
        {
            // TODO - process image
        }
        
        user.Update(req.FirstName, req.LastName, req.OtherNames, req.Email, photoUrl);
        await applicationDbContext.SaveChangesAsync(ct);

        await SendNoContentAsync(ct);
    }
}

public class UpdateUserProfileRequestValidator : Validator<UpdateUserProfileRequest>
{
    public UpdateUserProfileRequestValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.FirstName).NotEmpty().WithMessage("{PropertyName} is required.}");
        RuleFor(x => x.LastName).NotEmpty().WithMessage("{PropertyName} is required.");
        RuleFor(x => x.Email).NotEmpty().WithMessage("{PropertyName} is required.");
    }
}