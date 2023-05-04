using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Picator.Entities.Models;

namespace Picator.Entities.Mapping;

/// <summary>
/// Table mapping for refresh token.
/// </summary>
public class RefreshTokenMapping : BaseEntityTypeConfiguration<RefreshToken>
{
    public override void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.Property(p => p.ExpirationDate).HasColumnType("datetime2");
        builder.Property(p => p.Token).IsRequired();
        builder.Property(p => p.JwtId).IsRequired();
        base.Configure(builder);
    }
}