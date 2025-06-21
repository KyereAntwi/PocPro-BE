namespace DevSync.PocPro.Shops.StocksModule.Domains;

public class ProductMedia : BaseEntity<ProductMediaId>
{
    private ProductMedia() {}

    internal ProductMedia(ProductMediaType mediaType, string mediaUrl)
    {
        Id = ProductMediaId.Of(Guid.CreateVersion7());
        MediaType = mediaType;
        Url = mediaUrl;
        Status = StatusType.Active;
    }

    public ProductId ProductId { get; private set; }
    public ProductMediaType MediaType { get; private set; }
    public string Url { get; private set; }
}