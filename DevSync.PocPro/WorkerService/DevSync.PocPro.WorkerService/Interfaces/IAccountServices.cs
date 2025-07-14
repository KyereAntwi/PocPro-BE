namespace DevSync.PocPro.WorkerService.Interfaces;

public interface IAccountServices
{
    Task<IEnumerable<string>> GetAccountIdentifiersAsync(CancellationToken cancellationToken = default);
}