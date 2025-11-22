using LonelyInterview.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace LonelyInterview.Infrastructure.Data.EntityConfiguration;

internal class HrManagerConfiguration : IEntityTypeConfiguration<HrManager>
{
    public void Configure(EntityTypeBuilder<HrManager> builder)
    {
        builder.HasKey(x => x.Id);
    }
}
