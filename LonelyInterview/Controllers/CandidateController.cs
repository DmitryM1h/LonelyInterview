using LonelyInterview.Application.Requests.Candidate;
using LonelyInterview.Auth.Contracts;
using LonelyInterview.CodeExecution;
using LonelyInterview.Domain.Models;
using LonelyInterview.Infrastructure.Data.DataSources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LonelyInterview.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CandidateController(
    CandidateDataSource _candidateDatasource,
    LonelyInterviewUnitOfWork _unitOfWork,
    VacancyDataSource _vacancyDatasource,
    ResumeDataSource _resumeDatasource) : ControllerBase
{

    private Guid CandidateId 
    {
        get
        {
            var id = HttpContext.User.Claims.Where(t => t.Type == ClaimTypes.NameIdentifier).First().Value;
            return Guid.Parse(id);
        }
    }



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

        var candidate = await _candidateDatasource.GetCandidateWithInfo(CandidateId, token);

        if (candidate == null)
            return BadRequest("Candidate was not found");

        var info = CandidateInfo.Create(candidate.Id, updRequest.Specialty, updRequest.Degree, updRequest.GraduationYear, updRequest.WorkExperience);

        candidate.UpdateInfo(info);
        await _unitOfWork.SaveAsync(token);
        return Ok(info);
    }



    [HttpPost("AddResume")]
    [Authorize(Roles = nameof(Role.Candidate))]
    public async Task<ActionResult> AddResume([FromBody] AddResumeRequest addResumeReq, [FromQuery] Guid vacancyId, CancellationToken token = default)
    {

        var candidate = await _candidateDatasource.GetByIdOrDefaultAsync(CandidateId, token);

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

    [HttpGet("Resumes")]
    [Authorize(Roles = nameof(Role.Candidate))]
    public async Task<ActionResult> GetCandidatesResumes(CancellationToken token = default)
    {
 
        var resumes = await _resumeDatasource.GetByCandidateIdWithVacancyInfoAsync(CandidateId, token);

        return Ok(resumes.Select(t => new 
        { t.Vacancy.Title,
          t.Vacancy.ShortDescription,
          t.Status,
          t.CreatedAt,
          t.GitHubUrl,
          t.ActualSkills,
          t.PassiveSkills 
        }).ToList());

    }

    [HttpGet("AppliedVacancies")]
    public async Task<ActionResult> GetAppliedVacancies(CancellationToken token = default)
    {

        var resumes = await _candidateDatasource.GetAppliedResumesWithVacancyAsync(CandidateId, token);

        return Ok(resumes.Select(t => new { t.Vacancy.Title, t.Vacancy.Description}));
    }


    [HttpGet("Code")]
    public async Task<ActionResult> ExecuteCode([FromServices] CodeExecutionService codeService)
    {
        await codeService.CreateSubmission();
        return Ok();
    }

   


  
}
