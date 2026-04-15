using ChurchLiveScheduler.api.Models;
using Microsoft.EntityFrameworkCore;

namespace ChurchLiveScheduler.api.Configurations;

internal sealed class SpecialEntityTypeConfiguration : IEntityTypeConfiguration<Special>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Special> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .IsRequired();

        builder.Property(s => s.Datetime)
            .IsRequired()
            .HasConversion(v => v.ToUniversalTime(), v => DateTime.SpecifyKind(v, DateTimeKind.Utc)); ;
    }
}