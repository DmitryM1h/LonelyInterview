using LonelyInterview.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LonelyInterview.Infrastructure.Data.EntityConfiguration;

internal class CandidateInfoConfiguration : IEntityTypeConfiguration<CandidateInfo>
{
    public void Configure(EntityTypeBuilder<CandidateInfo> builder)
    {
        builder
            .HasKey(x => x.Id);

        builder
            .HasOne(t => t.Candidate)
            .WithOne(t => t.Info)
            .HasForeignKey<Candidate>()
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);
    }
}
