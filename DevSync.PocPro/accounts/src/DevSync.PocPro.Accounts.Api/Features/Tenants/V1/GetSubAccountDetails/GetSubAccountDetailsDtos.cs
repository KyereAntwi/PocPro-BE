namespace DevSync.PocPro.Accounts.Api.Features.Tenants.V1.GetSubAccountDetails;

public record GetSubAccountDetailsResponse(
    string BusinessName,
    string SettlementBank,
    string AccountNumber,
    float PercentageCharge);