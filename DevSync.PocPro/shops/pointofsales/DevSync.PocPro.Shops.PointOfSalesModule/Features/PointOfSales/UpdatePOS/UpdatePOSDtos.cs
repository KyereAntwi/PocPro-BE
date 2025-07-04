using Microsoft.AspNetCore.Mvc;

namespace DevSync.PocPro.Shops.PointOfSales.Features.PointOfSales.UpdatePOS;

public record UpdatePOSDto
{
    [FromRoute] public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Status { get; set; }
    public bool? OnlineEnabled { get; set; }
}