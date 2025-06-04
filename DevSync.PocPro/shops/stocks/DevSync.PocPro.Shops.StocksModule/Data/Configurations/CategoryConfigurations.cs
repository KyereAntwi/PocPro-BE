namespace DevSync.PocPro.Shops.StocksModule.Data.Configurations;

public class CategoryConfigurations : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(categoryId => categoryId.Value, dbId => CategoryId.Of(dbId));
        
        builder.Property(t => t.Status)
            .HasConversion(t => t.ToString(),
                dbType => Enum.Parse<StatusType>(dbType!));
    }
}