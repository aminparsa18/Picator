using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Picator.Entities.Models;

namespace Picator.Entities.Mapping;

/// <summary>
/// Table mapping for base entity.
/// </summary>
/// <typeparam name="TBase"></typeparam>
public class BaseEntityTypeConfiguration<TBase> : IEntityTypeConfiguration<TBase> where TBase : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<TBase> builder)
    {
        builder.Property(p => p.CreatedDate).HasColumnType("timestamp with time zone").HasDefaultValueSql("now()");
        builder.Property(p => p.ModifiedDate).HasColumnType("timestamp with time zone").HasDefaultValueSql("now()");
    }
}