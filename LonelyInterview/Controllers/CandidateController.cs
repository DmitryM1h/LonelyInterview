using LonelyInterview.Domain.Entities;
using LonelyInterview.Domain.Repository;
using Microsoft.AspNetCore.Mvc;

namespace LonelyInterview.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CandidateController(IRepository<Candidate, Guid> _candidateDatasource) : ControllerBase
    {

        [HttpGet]
        public async Task<IEnumerable<Candidate>> GetAllCandidate()
        {
           return await _candidateDatasource.GetAll();
        }


    }
}
