using LonelyInterview.Application.Requests.Hr;
using LonelyInterview.Auth.Contracts;
using LonelyInterview.Domain.Entities;
using LonelyInterview.Infrastructure.Data;
using LonelyInterview.Infrastructure.Data.DataSources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace LonelyInterview.Controllers;



[Route("api/[controller]")]
[ApiController]
public class HrManagerController(HrManagerDataSource _hrManagersdataSource, LonelyInterviewUnitOfWork _unitOfWork, LonelyInterviewContext _context) : ControllerBase
{

    [HttpGet("AllManagers")]
    public async Task<ActionResult<IEnumerable<HrManager>>> GetAllHrManagers()
    {
        var hrManagers =  await _hrManagersdataSource.GetAllAsync();
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

        await _context.SaveChangesAsync(token);

        await _unitOfWork.SaveAsync(token);

        return Ok();

    }

}
