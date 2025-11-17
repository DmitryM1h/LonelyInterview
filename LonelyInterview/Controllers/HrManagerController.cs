using LonelyInterview.Auth.Requests;
using LonelyInterview.Domain.Entities;
using LonelyInterview.Infrastructure.Data.DataSources;
using Microsoft.AspNetCore.Mvc;

namespace LonelyInterview.Controllers;



[Route("api/[controller]")]
[ApiController]
public class HrManagerController(HrManagerDataSource _hrManagersdataSource) : ControllerBase
{

    [HttpGet]
    public async Task<ActionResult<IEnumerable<HrManager>>> GetAllHrManagers()
    {
        var hrManagers =  await _hrManagersdataSource.GetAllAsync();
        return Ok(hrManagers);
    }

}
