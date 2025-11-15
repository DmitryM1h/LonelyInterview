using LonelyInterview.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LonelyInterview.Infrastructure.Data.EntityConfiguration;

internal class CandidateConfiguration : IEntityTypeConfiguration<Candidate>
{
    public void Configure(EntityTypeBuilder<Candidate> builder)
    {
        builder
            .HasKey(x => x.Id);

        builder
            .HasMany(x => x.Resumes)
            .WithOne(t => t.Candidate)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);

        //builder
        //    .HasOne(t => t.Info)
        //    .WithOne(t => t.Candidate)
        //    .HasForeignKey<Candidate>(t => t.InfoId)
        //    .OnDelete(DeleteBehavior.Cascade)
        //    .IsRequired(false);
    }
}
