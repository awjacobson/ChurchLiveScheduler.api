using ChurchLiveScheduler.api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChurchLiveScheduler.api.Configurations;

internal sealed class CancellationEntityTypeConfiguration : IEntityTypeConfiguration<Cancellation>
{
    public void Configure(EntityTypeBuilder<Cancellation> builder)
    {
        builder.HasKey(c => new { c.SeriesId, c.Date });
    }
}
