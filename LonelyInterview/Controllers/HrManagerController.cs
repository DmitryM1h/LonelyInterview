using LonelyInterview.Application.Requests.Hr;
using LonelyInterview.Auth.Contracts;
using LonelyInterview.Domain.Entities;
using LonelyInterview.Infrastructure.Data;
using LonelyInterview.Infrastructure.Data.DataSources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

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

        var vacancy = await _vacancyDataSource.GetByIdAsync(vacancyId, token);

        if (vacancy == null)
        {
            return NotFound("Vacancy not found.");
        }

           // Проверка, что текущий HR является создателем вакансии.
           if (vacancy.CreatedBy.Id != currHrIdGuid)
            {
                return Forbid("You are not authorized to close this vacancy. Only the creator can close it.");
            }

        if (!vacancy.isActive)
        {
            return BadRequest("Vacancy is already closed.");
        }

        vacancy.isActive = false; // Закрываем вакансию.
        await _unitOfWork.SaveAsync(token);


        return Ok("Vacancy closed successfully.");
    }


    [HttpGet("VacanciesCreatedByMe")]
    [Authorize(Roles = nameof(Role.HrManager))]
    public async Task<ActionResult<IEnumerable<Vacancy>>> GetVacanciesCreatedByMe(CancellationToken token = default)
    {
        var currHrId = HttpContext.User.Claims.Where(t => t.Type == ClaimTypes.NameIdentifier).First().Value;
        Guid currHrIdGuid = Guid.Parse(currHrId);

        var hrManager = await _hrManagersdataSource.GetByIdAsync(currHrIdGuid, token);
        if (hrManager == null)
        {
            return NotFound("HrManager not found.");
        }

        var vacancies = await _vacancyDataSource.Get(x => x.CreatedBy.Id == currHrIdGuid).ToListAsync(token); // Получаем вакансии, созданные текущим HR

        return Ok(vacancies);
    }
}
