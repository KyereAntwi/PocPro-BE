namespace DevSync.PocPro.Shops.Api.Utils;

public interface IMasterExtensions
{
    Task ApplyMigrationsAsync(CancellationToken cancellationToken = default);
}