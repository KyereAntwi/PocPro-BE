namespace DevSync.PocPro.Shops.Shared.Dtos;

public record ProductDto(Guid Id, string Name, string PhotoUrl)
{
    public Guid? PosId { get; set; } = null;
}