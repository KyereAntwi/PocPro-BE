using DevSync.PocPro.Shared.Domain.ValueObjects;

namespace DevSync.PocPro.Shops.PointOfSales.Data.Configurations;

public class SessionConfigurations : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(sessionId => sessionId.Value, dbId => SessionId.Of(dbId));
        
        builder.Property(a => a.PointOfSaleId).HasConversion(a => a.Value, dbId => PointOfSaleId.Of(dbId));
        
        builder.Property(t => t.Status)
            .HasConversion(t => t.ToString(),
                dbType => Enum.Parse<StatusType>(dbType!));
    }
}