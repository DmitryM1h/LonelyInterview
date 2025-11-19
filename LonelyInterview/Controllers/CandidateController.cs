using LonelyInterview.Application.Requests.Candidates;
using LonelyInterview.Auth.Contracts;
using LonelyInterview.Domain.Entities;
using LonelyInterview.Application.Requests.Candidate;
using LonelyInterview.Infrastructure.Data.DataSources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LonelyInterview.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CandidateController(CandidateDataSource _candidateDatasource, LonelyInterviewUnitOfWork _unitOfWork, VacancyDataSource _vacancyDatasource) : ControllerBase
{

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Candidate>>> GetAllCandidates()
    {
        var candidates = await _candidateDatasource.GetAllAsync();
        return Ok(candidates);
    }

    [HttpPost("UpdateInfo")]
    [Authorize(Roles = nameof(Role.Candidate))]
    public async Task<ActionResult> UpdateInfo([FromBody] UpdateCandidatesInfoRequest updRequest, CancellationToken token = default)
    {
        var currentUserId = HttpContext.User.Claims.Where(t => t.Type == ClaimTypes.NameIdentifier).First().Value;
        Guid currentUserIdGuid = Guid.Parse(currentUserId);

        // Проверяем, что CandidateId из запроса соответствует ID текущего пользователя
        if (updRequest.CandidateId != currentUserIdGuid)
        {
            return Forbid("You are not authorized to update information for other candidates.");
        }

        var cand = await _candidateDatasource.GetCandidateWithInfo(updRequest.CandidateId, token);
        if (cand == null)
            return BadRequest("Candidate was not found");

        var info = CandidateInfo.Create(updRequest.CandidateId, updRequest.Specialty, updRequest.Degree, updRequest.GraduationYear, updRequest.WorkExperience);

        cand.UpdateInfo(info);
        await _unitOfWork.SaveAsync(token);
        return Ok(info);
    }



    [HttpPost("AddResume")]
    [Authorize(Roles = nameof(Role.Candidate))]
    public async Task<ActionResult> AddResume([FromBody] AddResumeRequest addResumeReq, [FromQuery] Guid vacancyId, CancellationToken token = default)
    {
        var candidateId = HttpContext.User.Claims.Where(t => t.Type == ClaimTypes.NameIdentifier).First().Value;

        var candidate = await _candidateDatasource.GetByIdOrDefaultAsync(Guid.Parse(candidateId), token);

        if (candidate == null)
            return Forbid();

        var vacancy = await _vacancyDatasource.GetByIdOrDefaultAsync(vacancyId, token);

        if (vacancy == null || !vacancy.isActive)
        {
            return BadRequest("Specified vacancy does not exist");
        }

        var resume = Resume.Create(vacancy, addResumeReq.GitHubUrl, addResumeReq.ActualSkills, addResumeReq.PassiveSkills);

        candidate.AddResume(resume);

        await _unitOfWork.SaveAsync(token);

        return Ok(new { resume.Id, resume.Vacancy.ShortDescription, resume?.ActualSkills, resume?.PassiveSkills });

    }

    [HttpPost("Register")]  
    [AllowAnonymous] 
    public async Task<ActionResult> RegisterCandidate([FromBody] RegisterCandidateRequest request, CancellationToken token = default)
    {
        // 1. Валидация данных (здесь можно добавить более строгую валидацию)
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // 2. Проверка, не существует ли пользователь с таким email
        if (await _candidateDatasource.Get(x => x.Email == request.Email).AnyAsync(token))
        {
            return Conflict("A user with this email already exists."); //HTTP 409
        }

        // 3. Создание нового кандидата. Временная реализация
        var newCandidate = Candidate.Create(request.UserName, request.BirthDate, request.Speciality, request.GraduationYear, request.PhoneNumber, request.TelegramNick, request.Email, request.Password); 

        // 4. Сохранение кандидата в базе данных
        await _candidateDatasource.AddAsync(newCandidate, token);
        await _unitOfWork.SaveAsync(token);

        // 5. Возвращаем успешный результат
        return Ok("Candidate registered successfully.");
    }

}
