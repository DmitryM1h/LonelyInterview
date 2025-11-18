using LonelyInterview.Auth.Contracts;
using LonelyInterview.Domain.Entities;
using LonelyInterview.Application.Requests.Candidate;
using LonelyInterview.Domain.Requests;
using LonelyInterview.Infrastructure.Data.DataSources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LonelyInterview.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CandidateController(CandidateDataSource _candidateDatasource, LonelyInterviewUnitOfWork _unitOfWork
        ,VacancyDataSource _vacancyDatasource) : ControllerBase
    {

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Candidate>>> GetAllCandidates()
        {
           var candidates = await _candidateDatasource.GetAllAsync();
            return Ok(candidates);
        }

        [HttpPost("UpdateInfo")]
        public async Task<ActionResult> UpdateInfo([FromBody] UpdateCandidatesInfoRequest updRequest, CancellationToken token = default)
        {
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

    }
}
