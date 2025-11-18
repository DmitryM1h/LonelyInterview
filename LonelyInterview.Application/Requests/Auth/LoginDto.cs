using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LonelyInterview.Application.Requests.Auth
{
    public record LoginDto(string Email, string Password);
    
}
