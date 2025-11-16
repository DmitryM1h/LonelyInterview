using LonelyInterview.Domain.Entities;
using LonelyInterview.Domain.Repository;
using LonelyInterview.Infrastructure.Data;
using LonelyInterview.Infrastructure.Data.DataSources;
using Microsoft.AspNetCore.Mvc;

namespace LonelyInterview.Controllers;



[Route("api/[controller]")]
[ApiController]
public class HrManagerController(HrManagerDataSource _hrManagersdataSource) : ControllerBase
{

    [HttpGet]
    public async Task<IEnumerable<HrManager>> GetAllHrManagers()
    {
        return await _hrManagersdataSource.GetAllAsync();
    }

 
}
