namespace DevSync.PocPro.Shops.PromoCodesModule.Data.Configurations;

public class PromoCodeConfiguration : IEntityTypeConfiguration<PromoCode>
{
    public void Configure(EntityTypeBuilder<PromoCode> builder)
    {
        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.CodeType)
            .IsRequired()
            .HasDefaultValue(CodeType.SingleUsePerUser)
            .HasConversion(t => t.ToString(),
                dbType => Enum.Parse<CodeType>(dbType!));
        
        builder
            .HasMany(x => x.PromoCodeUsers)
            .WithOne()
            .HasForeignKey(x => x.PromoCodeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}