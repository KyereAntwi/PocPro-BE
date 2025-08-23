namespace DevSync.PocPro.Shops.Shared.Interfaces;

public interface IPaymentServices
{
    Task<bool> VerifyAsync(string paymentRef, CancellationToken token = default);
    Task<(string, string, string)> InitializeAsync(decimal amount, string customerEmail, string subAccount, CancellationToken token = default);
    Task<bool> VerifyHttpAsync(string paymentRef, CancellationToken token = default);
    Task<(string, string, string)> InitializeHttpAsync(decimal amount, string customerEmail, string subAccount, CancellationToken token = default);
}