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
        
        session.ClosedBy = userId;
        session.ClosedAt = DateTimeOffset.Now;
        return Result.Ok();
    }
    
    public string Title { get; private set; } = string.Empty;
}