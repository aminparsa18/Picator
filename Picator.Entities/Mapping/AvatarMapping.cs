using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Picator.Entities.Models;

namespace Picator.Entities.Mapping;

/// <summary>
/// Table mapping for avatar.
/// </summary>
public class AvatarMapping : BaseEntityTypeConfiguration<Avatar>
{
    public override void Configure(EntityTypeBuilder<Avatar> builder)
    {
        builder.Property(p => p.Name).IsRequired();
        base.Configure(builder);
    }
}