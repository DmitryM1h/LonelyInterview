using LonelyInterview.Domain.Entities;
using LonelyInterview.Domain.Repository;
using LonelyInterview.Infrastructure.Data.DataSources;
using Microsoft.AspNetCore.Mvc;

namespace LonelyInterview.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CandidateController(CandidateDataSource _candidateDatasource) : ControllerBase
    {

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Candidate>>> GetAllCandidates()
        {
           var candidates = await _candidateDatasource.GetAllAsync();
            return Ok(candidates);
        }
    }
}
