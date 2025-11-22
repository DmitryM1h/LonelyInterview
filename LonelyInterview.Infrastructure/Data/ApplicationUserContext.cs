using LonelyInterview.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;


namespace LonelyInterview.Infrastructure.Data
{
    public class ApplicationUserContext(DbContextOptions<ApplicationUserContext> options) : IdentityDbContext<
    ApplicationUser,
    IdentityRole<Guid>,
    Guid,
    IdentityUserClaim<Guid>,
    IdentityUserRole<Guid>,
    IdentityUserLogin<Guid>,
    IdentityRoleClaim<Guid>,
    IdentityUserToken<Guid>>(options)
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);
            builder.HasDefaultSchema("Auth");

            //builder
            //    .Entity<IdentityRole<Guid>>()
            //    .HasData(new IdentityRole<Guid>
            //    {
            //        Id = Guid.NewGuid(),
            //        Name = "Administrator",
            //    },
            //new IdentityRole<Guid>
            //{
            //    Id = Guid.NewGuid(),
            //    Name = "HrManager",
            //},
            //new IdentityRole<Guid>
            //{
            //    Id = Guid.NewGuid(),
            //    Name = "Candidate",

            //});
        }

        public override DbSet<ApplicationUser> Users { get; set; } = null!;



        

        public static async Task SeedRolesAsync(RoleManager<IdentityRole<Guid>> roleManager)
        {
            string[] roles = { "Administrator", "HrManager", "Candidate"};

            foreach (var roleName in roles)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
                }
            }
        }

    }


}
