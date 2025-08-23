namespace DevSync.PocPro.Accounts.Api.Features.Tenants.V1.CreateSubAccount;

public record CreateSubAccountRequest(
    string BusinessName,
    string SettlementBank,
    string AccountNumber);