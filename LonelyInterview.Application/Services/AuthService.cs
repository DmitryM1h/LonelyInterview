using LonelyInterview.Application.Requests.Auth;
using LonelyInterview.Auth;
using LonelyInterview.Domain.Models;
using LonelyInterview.Infrastructure.Data.DataSources;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;


namespace LonelyInterview.Application.Services
{

    public record Result<T>(bool isSuccess, string? error, T? result);
    public record Result(bool isSuccess, string? error);


    public class AuthService(SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        TokenGenerator tokenGenerator,
        CandidateDataSource _candidateRepo,
        HrManagerDataSource _hrManagerRepo,
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

            var claims = await GetUserClaimsAsync(user);

            var token = tokenGenerator.GenerateToken(claims);

            return new Result<string>(true, null, token);

        }


        public async Task<Result> RegisterCandidateAsync(RegisterCandidateDto regRequest, CancellationToken token)
        {
            //TODO Транзакция
            var user = ApplicationUser.CreateFromRegisterDto(
                                                        UserName: regRequest.UserName,
                                                        BirthDay: regRequest.BirthDay,
                                                        Email: regRequest.Email,
                                                        Telegram: regRequest.Telegram,
                                                        Password: regRequest.Password,
                                                        PhoneNumber: regRequest.PhoneNumber
                                                    );

            var result = await userManager.CreateAsync(user, regRequest.Password);

            if (!result.Succeeded)
                return new Result(false, result.Errors.First().Description.ToString());

            await AddToRole(user, nameof(Candidate));


            var cand = Candidate.CreateCandidate(user.Id, CandidateInfo.Create(user.Id, regRequest.Specialty, regRequest.Degree, regRequest.GraduationYear, regRequest.WorkExperience));

            await _candidateRepo.AddAsync(cand, token);

            await _unitOfWork.SaveAsync(token);

            return new Result(true, null);
        }

        public async Task<Result> RegisterHrManagerAsync(RegisterHrDto regRequest, CancellationToken token)
        {
            //TODO Транзакция
            var user = ApplicationUser.CreateFromRegisterDto(
                                                        UserName: regRequest.UserName,
                                                        BirthDay: regRequest.BirthDay,
                                                        Email: regRequest.Email,
                                                        Telegram: regRequest.Telegram,
                                                        Password: regRequest.Password,
                                                        PhoneNumber: regRequest.PhoneNumber
                                                    );

            var result = await userManager.CreateAsync(user, regRequest.Password);

            if (!result.Succeeded)
                return new Result(false, result.Errors.First().Description.ToString());

            await AddToRole(user, nameof(HrManager));


            var hr = HrManager.CreateHr(user.Id, regRequest.Company);

            await _hrManagerRepo.AddAsync(hr, token);

            await _unitOfWork.SaveAsync(token);
            
            return new Result(true, null);
        }


        private async Task AddToRole(ApplicationUser user, string Role)
        {
            await userManager.AddToRoleAsync(user, Role);
            await userManager.AddClaimAsync(user, new Claim("email", user!.Email!));
            await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Name, user.UserName!));
            await userManager.AddClaimAsync(user, new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            //await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, Role));
            await userManager.AddClaimAsync(user, new Claim("sub", user.Id.ToString()));
        }

        private async Task<List<Claim>> GetUserClaimsAsync(ApplicationUser user)
        {
            var roles = await userManager.GetRolesAsync(user);
            var userClaims = await userManager.GetClaimsAsync(user);

            var claims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();
           // var doubledRoles = roles.Select(role => new Claim("role", role)).ToList();
            claims.AddRange(userClaims);
          //  claims.AddRange(doubledRoles);

            return claims;
        }
    }
}
