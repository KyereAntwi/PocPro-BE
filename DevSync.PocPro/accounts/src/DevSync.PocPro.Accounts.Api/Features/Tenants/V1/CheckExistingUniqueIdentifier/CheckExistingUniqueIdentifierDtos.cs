namespace DevSync.PocPro.Accounts.Api.Features.Tenants.V1.CheckExistingUniqueIdentifier;

public record CheckExistingUniqueIdentifierRequest([FromRoute] string UniqueIdentifier);

public record CheckExistingUniqueIdentifierResponse(bool UniqueIdentifierExists);