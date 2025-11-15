using LonelyInterview.Domain.Entities;


namespace LonelyInterview.Infrastructure.Data.DataSources;

public class HrManagerDataSource(LonelyInterviewContext context) : AbstractRepository<HrManager, Guid>(context)
{


}
