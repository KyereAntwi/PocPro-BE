using DevSync.PocPro.Shared.Domain.ValueObjects;

namespace DevSync.PocPro.Shops.PointOfSales.Domains;

public class PointOfSale : BaseEntity<PointOfSaleId>
{
    private readonly Collection<Session> _sessions = [];
    public IReadOnlyCollection<Session> Sessions => _sessions;
    
    public static PointOfSale Create(string title)
    {
        var pos = new PointOfSale
        {
            Id = PointOfSaleId.Of(Guid.CreateVersion7()),
            Title = title
        };

        return pos;
    }

    public Result<Guid> StartSession()
    {
        var activeSession = _sessions.FirstOrDefault(s => s.ClosedAt == null);

        if (activeSession != null)
        {
            return Result.Fail("There is an active session. You cannot start a new one.");
        }
        
        var newSession = new Session(Id);
        _sessions.Add(newSession);
        return Result.Ok(newSession.Id.Value);
    }

    public Result EndSession(Guid sessionId, string userId)
    {
        var session = _sessions.FirstOrDefault(s => s.Id == SessionId.Of(sessionId));

        if (session == null)
        {
            return Result.Fail("Session not found");
        }
        
        if (session.ClosedAt != null)
        {
            return Result.Fail("Session already closed");
        }
        
        session.ClosedBy = userId;
        session.ClosedAt = DateTimeOffset.Now;
        return Result.Ok();
    }
    
    public string Title { get; private set; } = string.Empty;
}