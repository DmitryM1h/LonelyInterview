using LonelyInterview.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace LonelyInterview.Infrastructure.Data;

public class LonelyInterviewContext : DbContext
{
    public LonelyInterviewContext(DbContextOptions<LonelyInterviewContext> options) : base(options) { }


    public DbSet<Candidate> Candidates { get; set; }
    //public DbSet<CandidateInfo> CandidatesInfo { get; set; }
    public DbSet<HrManager> HrManagers { get; set; }
    public DbSet<Resume> Resumes { get; set; }
    public DbSet<Vacancy> Vacancies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("LonelyInterview");
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);

    }

}
