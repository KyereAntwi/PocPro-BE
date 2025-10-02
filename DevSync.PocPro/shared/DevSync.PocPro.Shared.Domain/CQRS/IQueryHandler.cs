namespace DevSync.PocPro.Shared.Domain.CQRS;

public interface IQueryHandler<in TQuery, TResult> where TResult: notnull
{
    Task<Result<TResult>> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}