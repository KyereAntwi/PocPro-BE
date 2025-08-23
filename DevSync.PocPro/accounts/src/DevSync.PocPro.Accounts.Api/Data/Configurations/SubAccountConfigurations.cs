namespace DevSync.PocPro.Accounts.Api.Data.Configurations;

public class SubAccountConfigurations : IEntityTypeConfiguration<SubAccount>
{
    public void Configure(EntityTypeBuilder<SubAccount> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, dbId => SubAccountId.Of(dbId));

        builder.Property(x => x.AccountNumber)
            .IsRequired();
        
        builder.Property(a => a.TenantId).HasConversion(a => a!.Value, dbId => TenantId.Of(dbId));
        builder.HasIndex(a => a.TenantId);
    }
}