using LonelyInterview.Domain.Entities;
using LonelyInterview.Domain.Repository;
using LonelyInterview.Domain.Requests;
using LonelyInterview.Infrastructure.Data.DataSources;
using Microsoft.AspNetCore.Mvc;

namespace LonelyInterview.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CandidateController(CandidateDataSource _candidateDatasource, LonelyInterviewUnitOfWork _unitOfWork) : ControllerBase
    {

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Candidate>>> GetAllCandidates()
        {
           var candidates = await _candidateDatasource.GetAllAsync();
            return Ok(candidates);
        }

        [HttpPost("UpdateInfo")]
        public async Task<ActionResult> UpdateCandidateInfo([FromBody] UpdateCandidatesInfoRequest updRequest, CancellationToken token = default)
        {
            var cand = await _candidateDatasource.GetCandidateWithInfo(updRequest.CandidateId, token);
            if (cand == null)
                return BadRequest("Candidate was not found");

            var info = CandidateInfo.Create(updRequest.CandidateId, updRequest.Specialty, updRequest.Degree, updRequest.GraduationYear, updRequest.WorkExperience);

            cand.UpdateInfo(info);
            await _unitOfWork.SaveAsync(token);
            return Ok(info);
        }



        //[HttpPost("AddResume")]
        //public async Task<ActionResult> UpdateCandidateInfo([FromBody] UpdateCandidatesInfoRequest updRequest, CancellationToken token = default)
        //{


        //}

    }
}
