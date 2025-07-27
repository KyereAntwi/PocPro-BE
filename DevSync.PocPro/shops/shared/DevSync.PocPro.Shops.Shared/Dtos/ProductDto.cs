namespace DevSync.PocPro.Shops.Shared.Dtos;

public record ProductDto(Guid Id, string Name, string PhotoUrl, decimal Price)
{
    public Guid? PosId { get; set; } = null;
}