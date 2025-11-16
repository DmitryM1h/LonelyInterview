using LonelyInterview.Auth;
using LonelyInterview.Auth.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LonelyInterview.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController(RoleManager<IdentityRole<Guid>> roleManager,
                                UserManager<ApplicationUser> manager
                               ) : ControllerBase
    {
        [HttpPost("CreateRole")]
        //[Authorize(Roles = nameof(Role.Administrator))]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (!Enum.TryParse<Role>(roleName, out _))
                return BadRequest("Invalid role");

            if (!await roleManager.RoleExistsAsync(roleName))
                return Ok();

            await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
            return Ok();
        }


        [HttpPost("AddToRole")]
        //[Authorize(Roles = nameof(Role.Administrator))]
        public async Task<IActionResult> AddToRole(string email, string roleName)
        {
            if (!Enum.TryParse<Role>(roleName, true, out Role role))
                return BadRequest("Invalid role");

            var user = await manager.FindByEmailAsync(email);
            if (user is null)
                return BadRequest("User does not exist");

            var action = await manager.AddToRoleAsync(user, roleName);
            //var addClaims = await manager.AddClaimAsync(user,new System.Security.Claims.Claim { })

            if (!action.Succeeded)
                return BadRequest(action.Errors);


            return Ok();
        }
    }
}
