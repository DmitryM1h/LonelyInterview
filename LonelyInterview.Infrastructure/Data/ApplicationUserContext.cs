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
        }

        public override DbSet<ApplicationUser> Users { get; set; } = null!;



    }


}
