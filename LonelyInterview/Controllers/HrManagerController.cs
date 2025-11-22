using LonelyInterview.Application.Requests.Hr;
using LonelyInterview.Auth.Contracts;
using LonelyInterview.Domain.Models;
using LonelyInterview.Infrastructure.Data.DataSources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LonelyInterview.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HrManagerController(HrManagerDataSource _hrManagersdataSource, LonelyInterviewUnitOfWork _unitOfWork, VacancyDataSource _vacancyDataSource) : ControllerBase
{

    [HttpGet("AllManagers")]
    public async Task<ActionResult<IEnumerable<HrManager>>> GetAllHrManagers()
    {
        var hrManagers = await _hrManagersdataSource.GetAllAsync();
        return Ok(hrManagers);
    }


    [HttpPost("CreateVacancy")]
    [Authorize(Roles = nameof(Role.HrManager))]
    public async Task<ActionResult> CreateVacation(CreateVacancyRequest request, CancellationToken token = default)
    {
        var currHrId = HttpContext.User.Claims.Where(t => t.Type == ClaimTypes.NameIdentifier).First().Value;

        var currHr = await _hrManagersdataSource.GetByIdOrDefaultAsync(Guid.Parse(currHrId), token);

        if (currHr == null)
            return Forbid();

        Vacancy vacancy = Vacancy.Create(request.Title, request.Description, request.RequiredSkills, request.Location, currHr,
            request.ShortDescription, request.NiceToHaveSkills, request.MinYearsOfExperience, request.SalaryFrom, request.SalaryTo,
            request.Currency);

        currHr.CreateVacancy(vacancy);

        await _unitOfWork.SaveAsync(token);

        return Ok();

    }

    [HttpPut("CloseVacancy/{vacancyId}")]
    [Authorize(Roles = nameof(Role.HrManager))]
    public async Task<ActionResult> CloseVacancy(Guid vacancyId, CancellationToken token = default)
    {
        var currHrId = HttpContext.User.Claims.Where(t => t.Type == ClaimTypes.NameIdentifier).First().Value;
        Guid currHrIdGuid = Guid.Parse(currHrId);

        var vacancy = await _vacancyDataSource.GetByIdOrDefaultAsync(vacancyId, token);

        if (vacancy == null)
        {
            return NotFound("Vacancy not found.");
        }

        if (vacancy.ResponsibleHr.Id != currHrIdGuid)
        {
            return Forbid("Access denied");
        }

        if (vacancy.Status == VacancyStatus.Closed)
        {
            return Ok();
        }

        vacancy.SetStatus(VacancyStatus.Closed);

        await _unitOfWork.SaveAsync(token);


        return Ok();
    }


    [HttpGet("HrManagersVacancies")]
    [Authorize(Roles = nameof(Role.HrManager))]
    public async Task<ActionResult<IEnumerable<Vacancy>>> GetHrManagersVacancies(CancellationToken token = default)
    {
        var currHrId = HttpContext.User.Claims.Where(t => t.Type == ClaimTypes.NameIdentifier).First().Value;

        if (currHrId is null)
            return Unauthorized();

        var vacancies = await _vacancyDataSource.GetVacanciesByHrIdAsync(Guid.Parse(currHrId), token);

        return Ok(vacancies);
    }
}
