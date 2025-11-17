using LonelyInterview.Auth;
using LonelyInterview.Auth.Requests;
using LonelyInterview.Domain.Entities;
using LonelyInterview.Domain.Repository;
using LonelyInterview.Infrastructure.Data.DataSources;
using Microsoft.AspNetCore.Identity;
using System.Net;
using System.Security.Claims;


namespace LonelyInterview.Application.Services
{

    public record Result<T>(bool isSuccess, string? error, T? result);
    public record Result(bool isSuccess, string? error);


    public class AuthService(SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        TokenGenerator tokenGenerator,
        CandidateDataSource _candidateRepo,
        LonelyInterviewUnitOfWork _unitOfWork)
    {

        public async Task<Result<string>> LoginAsync(LoginDto logDto)
        {
            var user = await userManager.FindByEmailAsync(logDto.Email);

            if (user is null)
                return new Result<string>(false, "Invalid login or password", null);


            var p = await signInManager.CheckPasswordSignInAsync(user, logDto.Password, false);

            if (!p.Succeeded)
                return new Result<string>(false, "Invalid login or password", null);

            var roles = await userManager.GetRolesAsync(user);
            var userClaims = await userManager.GetClaimsAsync(user);

            var claims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();

            claims.AddRange(userClaims);

            var token = tokenGenerator.GenerateToken(claims);

            return new Result<string>(true, null, token);

        }


        public async Task<Result> RegisterCandidateAsync(RegisterCandidateDto regRequest, CancellationToken token)
        {
            //TODO Транзакция
            var user = ApplicationUser.CreateFromRegisterDto(regRequest);

            var result = await userManager.CreateAsync(user, regRequest.Password);

            if (!result.Succeeded)
                return new Result(false, result.Errors.First().Description.ToString());

            await userManager.AddToRoleAsync(user, nameof(Candidate));
            await userManager.AddClaimAsync(user, new Claim("email", regRequest.Email));
            await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Name, regRequest.UserName));


            var cand = Candidate.CreateCandidate(user.Id, CandidateInfo.Create(user.Id, regRequest.Specialty, regRequest.Degree, regRequest.GraduationYear, regRequest.WorkExperience));

            await _candidateRepo.AddAsync(cand, token);

            await _unitOfWork.SaveAsync(token);

            return new Result(true, null);
        }


    }
}
