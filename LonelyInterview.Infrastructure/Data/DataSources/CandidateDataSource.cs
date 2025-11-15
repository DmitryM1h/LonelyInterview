using LonelyInterview.Domain.Entities;
using LonelyInterview.Domain.Repository;


namespace LonelyInterview.Infrastructure.Data.DataSources;

public class CandidateDataSource(LonelyInterviewContext context) : AbstractRepository<Candidate, Guid>(context)
{


}
