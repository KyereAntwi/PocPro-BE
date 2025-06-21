using DevSync.PocPro.Shared.Domain.ValueObjects;
using DevSync.PocPro.Shops.Shared.ValueObjects;

namespace DevSync.PocPro.Shops.PointOfSales.Domains;

public class PointOfSale : BaseEntity<PointOfSaleId>
{
    private readonly Collection<Session> _sessions = [];
    public IReadOnlyCollection<Session> Sessions => _sessions;
    
    private readonly Collection<PointOfSaleManager> _managers = [];
    public IReadOnlyCollection<PointOfSaleManager> Managers => _managers;
    
    public static PointOfSale Create(string title, string phone, string address, string email)
    {
        var pos = new PointOfSale
        {
            Id = PointOfSaleId.Of(Guid.CreateVersion7()),
            Title = title,
            Phone = phone,
            Address = address,
            Email = email
        };

        return pos;
    }
    
    public void Update(string title, string phone, string address, string email, StatusType? status)
    {
        Title = title;
        Phone = phone;
        Address = address;
        Email = email;
        Status = status;
    }

    public Result<Guid> StartSession(double openingCash)
    {
        var activeSession = _sessions.FirstOrDefault(s => s.ClosedAt == null);

        if (activeSession != null)
        {
            return Result.Fail("There is an active session. You cannot start a new one.");
        }
        
        if (openingCash < 0)
        {
            return Result.Fail("Opening cash cannot be negative");
        }
        
        var newSession = new Session(Id, openingCash);
        _sessions.Add(newSession);
        return Result.Ok(newSession.Id.Value);
    }

    public Result EndSession(Guid sessionId, string userId, double closingCash)
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
        
        session.EndSession(userId, closingCash);
        return Result.Ok();
    }
    
    public Result<Guid> AddManager(string userId)
    {
        var existingManager = _managers.FirstOrDefault(m => m.UserId == userId);

        if (existingManager != null)
        {
            return Result.Fail("User is already a manager");
        }

        var newManager = new PointOfSaleManager(Id, userId);
        _managers.Add(newManager);
        return Result.Ok(newManager.Id.Value);
    }
    
    public Result RemoveManager(string userId)
    {
        var manager = _managers.FirstOrDefault(m => m.UserId == userId);

        if (manager == null)
        {
            return Result.Fail("Manager not found");
        }

        _managers.Remove(manager);
        return Result.Ok();
    }
    
    public string Title { get; private set; } = string.Empty;
    public string? Email { get; private set; }
    public string? Address { get; private set; }
    public string? Phone { get; private set; }
}