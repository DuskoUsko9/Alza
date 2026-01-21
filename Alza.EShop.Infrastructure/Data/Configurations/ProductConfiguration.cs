using Alza.EShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Alza.EShop.Infrastructure.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.ImageUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(p => p.Price)
            .HasPrecision(18, 2);

        builder.Property(p => p.Description)
            .HasMaxLength(2000);

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .IsRequired();

        builder.HasIndex(p => p.Name);
        builder.HasIndex(p => p.Price);
    }
}
