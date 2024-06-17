using ChurchLiveScheduler.api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChurchLiveScheduler.api.Configurations;

internal sealed class SeriesEntityTypeConfiguration : IEntityTypeConfiguration<Series>
{
    public void Configure(EntityTypeBuilder<Series> builder)
    {
        builder.HasKey(s => new { s.Id });

        builder.HasMany(s => s.Cancellations);
    }
}
