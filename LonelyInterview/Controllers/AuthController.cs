using LonelyInterview.Application.Services;
using LonelyInterview.Auth.Requests;

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



        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterCandidateDto regRequest, CancellationToken token = default)
        {

            var result = await _authService.RegisterCandidateAsync(regRequest, token);

            if (!result.isSuccess) return BadRequest(result.error);

            return Ok();
        }

    }
}
