namespace DevSync.PocPro.WorkerService.Interfaces;

public interface IPosServices
{
    Task<IReadOnlyList<Guid>> GetAllPosIdsAsync(string tenantIdentifier, CancellationToken cancellationToken = default);
}