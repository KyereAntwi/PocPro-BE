namespace DevSync.PocPro.Accounts.Api.Features.ApplicationUsers.V1.AddApplicationUserViaOnBoarding;

public class AddApplicationUserViaOnBoardingEndpoint(
    IHttpContextAccessor httpContextAccessor,
    ApiAuthentication apiAuthentication,
    Shared.Domain.CQRS.ICommandHandler<AddApplicationUserViaOnBoardingRequest, Guid> handler) 
    : Endpoint<AddApplicationUserViaOnBoardingRequest>
{
    public override void Configure()
    {
        Post("/api/v1/accounts/users/via-onboarding");
        AllowAnonymous();
    }

    public override async Task HandleAsync(AddApplicationUserViaOnBoardingRequest req, CancellationToken ct)
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
            await SendAsync(new BaseResponse<string>("Failed to add application user", false)
            {
                Errors = result.Errors.Select(e => e.Message).ToList()
            }, StatusCodes.Status400BadRequest, ct);
            return;
        }

        await SendNoContentAsync(ct);
    }
}
