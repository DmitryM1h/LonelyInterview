using LonelyInterview.Auth;
using LonelyInterview.Auth.Requests;
using LonelyInterview.Domain.Entities;
using LonelyInterview.Infrastructure.Data.DataSources;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Npgsql;
using System.Data;
using System.Security.Claims;

namespace LonelyInterview.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        CandidateDataSource _candidateRepo,
        UnitOfWork _unitOfWork,
        TokenGenerator tokenGenerator) : ControllerBase
    {

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] LoginDto logDto)
        {

            var user = await userManager.FindByEmailAsync(logDto.Email);

            if (user is null)
                return NotFound($"Invalid login or password");


            var p = await signInManager.CheckPasswordSignInAsync(user, logDto.Password, false);

            if (!p.Succeeded)
                return BadRequest("Invalid login or password");

            var roles = await userManager.GetRolesAsync(user);
            var userClaims = await userManager.GetClaimsAsync(user);

            var claims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();

            claims.AddRange(userClaims);

            var token = tokenGenerator.GenerateToken(claims);

            HttpContext.Response.Cookies.Append("JWT", token);


            return Ok(token);
        }



        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto regRequest, CancellationToken token = default)
        {

            var user = ApplicationUser.CreateFromRegisterDto(regRequest);

            var result = await userManager.CreateAsync(user, regRequest.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors.ToString());

            await userManager.AddToRoleAsync(user, nameof(Candidate));
            await userManager.AddClaimAsync(user, new Claim("email", regRequest.Email));
            await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Name, regRequest.UserName));
            

            var cand = Candidate.CreateCandidate(user.Id, CandidateInfo.Create(user.Id, regRequest.Specialty, regRequest.Degree, regRequest.GraduationYear, regRequest.WorkExperience));

            await _candidateRepo.AddAsync(cand, token);

            await _unitOfWork.SaveAsync(token);

            return Ok();
        }

    }
}
