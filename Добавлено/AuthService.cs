using LonelyInterview.Application.Requests.Auth;
using LonelyInterview.Auth;
using LonelyInterview.Domain.Entities;
using LonelyInterview.Infrastructure.Data.DataSources;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace LonelyInterview.Application.Services
{
    public record Result<T>(bool isSuccess, string? error, T? result);
    public record Result(bool isSuccess, string? error);

    public class AuthService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TokenGenerator _tokenGenerator;
        private readonly CandidateDataSource _candidateRepo;
        private readonly HrManagerDataSource _hrManagerRepo;
        private readonly LonelyInterviewUnitOfWork _unitOfWork;

        public AuthService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, TokenGenerator tokenGenerator, CandidateDataSource candidateRepo, HrManagerDataSource hrManagerRepo, LonelyInterviewUnitOfWork unitOfWork)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _tokenGenerator = tokenGenerator;
            _candidateRepo = candidateRepo;
            _hrManagerRepo = hrManagerRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<string>> LoginAsync(LoginDto logDto)
        {
            var user = await _userManager.FindByEmailAsync(logDto.Email);

            if (user == null)
                return new Result<string>(false, "Invalid login or password", null);

            var p = await _signInManager.CheckPasswordSignInAsync(user, logDto.Password, false);

            if (!p.Succeeded)
                return new Result<string>(false, "Invalid login or password", null);

            // Получаем Claims пользователя
            var claims = await GetUserClaimsAsync(user);

            // Генерируем токен, используя Claims
            var token = _tokenGenerator.GenerateToken(claims);

            return new Result<string>(true, null, token);
        }

        public async Task<Result> RegisterCandidateAsync(RegisterCandidateDto regRequest, CancellationToken token)
        {
            //TODO Транзакция
            var user = ApplicationUser.CreateFromRegisterDto(
                                                        regRequest.UserName,
                                                        regRequest.BirthDay,
                                                        regRequest.Email,
                                                        regRequest.Telegram,
                                                        regRequest.Password
                                                    );

            var result = await _userManager.CreateAsync(user, regRequest.Password);

            if (!result.Succeeded)
                return new Result(false, result.Errors.First().Description);

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
                                                        regRequest.UserName,
                                                        regRequest.BirthDay,
                                                        regRequest.Email,
                                                        regRequest.Telegram,
                                                        regRequest.Password
                                                    );

            var result = await _userManager.CreateAsync(user, regRequest.Password);

            if (!result.Succeeded)
                return new Result(false, result.Errors.First().Description);

            await AddToRole(user, nameof(HrManager));

            var hr = HrManager.CreateHr(user.Id, regRequest.Company);

            await _hrManagerRepo.AddAsync(hr, token);

            await _unitOfWork.SaveAsync(token);

            return new Result(true, null);
        }


        private async Task AddToRole(ApplicationUser user, string Role)
        {
            await _userManager.AddToRoleAsync(user, Role);
            await _userManager.AddClaimAsync(user, new Claim("email", user.Email));
            await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Name, user.UserName));
            await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            await _userManager.AddClaimAsync(user, new Claim("sub", user.Id.ToString()));
        }

        private async Task<List<Claim>> GetUserClaimsAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var userClaims = await _userManager.GetClaimsAsync(user);

            var claims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();
            claims.AddRange(userClaims);

            return claims;
        }
    }
}
