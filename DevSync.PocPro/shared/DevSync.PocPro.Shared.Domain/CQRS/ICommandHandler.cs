namespace DevSync.PocPro.Shared.Domain.CQRS;

public interface ICommandHandler<in TCommand, TResult> where TCommand: notnull
{
    Task<Result<TResult>> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}