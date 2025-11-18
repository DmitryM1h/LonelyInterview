using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LonelyInterview.Application.Requests.Candidate
{
    public record AddResumeRequest(string? GitHubUrl = null, string? ActualSkills = null, string? PassiveSkills = null);

}
