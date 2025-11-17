

namespace LonelyInterview.Domain.Requests
{
    public record UpdateCandidatesInfoRequest
    {
        public Guid CandidateId { get; init; }
        public string? Specialty { get; init; }
        public string? Degree { get; init; }
        public int? GraduationYear { get; init; }
        public string? WorkExperience { get; init; }
    }
}
