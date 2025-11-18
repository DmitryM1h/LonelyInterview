using LonelyInterview.Application.Services;
using LonelyInterview.Application.Requests;

using Microsoft.AspNetCore.Mvc;

namespace LonelyInterview.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(
        AuthService _authService
       ) : ControllerBase
    {

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] LoginDto logDto)
        {

            var Result = await _authService.LoginAsync(logDto);

            if (!Result.isSuccess)
                return BadRequest(Result.error);

            var token = Result.result;
            //HttpContext.Response.Cookies.Append("JWT", token!);


            return Ok(token);
        }



        [HttpPost("Candidate/Register")]
        public async Task<IActionResult> RegisterCandidate([FromBody] RegisterCandidateDto regRequest, CancellationToken token = default)
        {

            var result = await _authService.RegisterCandidateAsync(regRequest, token);

            if (!result.isSuccess) return BadRequest(result.error);

            return Ok();
        }


        [HttpPost("HrManager/Register")]
        public async Task<IActionResult> RegisterHr([FromBody] RegisterHrDto regRequest, CancellationToken token = default)
        {

            var result = await _authService.RegisterHrManagerAsync(regRequest, token);

            if (!result.isSuccess) return BadRequest(result.error);

            return Ok();
        }

    }
}
