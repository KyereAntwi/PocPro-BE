using Microsoft.AspNetCore.Mvc;

namespace DevSync.PocPro.Shops.PointOfSales.Features.PointOfSales.UpdatePOS;

public record UpdatePOSRequest([FromRoute] Guid Id, [Microsoft.AspNetCore.Mvc.FromBody] UpdatePOSDto UpdatePOSDto);

public record UpdatePOSDto
{
    public string Title { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}