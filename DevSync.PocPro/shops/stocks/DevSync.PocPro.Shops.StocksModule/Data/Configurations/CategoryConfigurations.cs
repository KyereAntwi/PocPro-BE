namespace DevSync.PocPro.Shops.StocksModule.Data.Configurations;

public class CategoryConfigurations : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(categoryId => categoryId.Value, dbId => CategoryId.Of(dbId));
    }
}