using LonelyInterview.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace LonelyInterview.Infrastructure.Data.EntityConfiguration;

internal class VacancyConfiguration : IEntityTypeConfiguration<Vacancy>
{
    public void Configure(EntityTypeBuilder<Vacancy> builder)
    {
        builder
            .HasKey(t => t.Id);

     

        builder
            .HasOne(t => t.ResponsibleHr)
            .WithMany(t => t.Vacancies)
            .IsRequired(true);

        builder
            .HasMany(t => t.Resumes)
            .WithOne(t => t.Vacancy)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);
    }
}
