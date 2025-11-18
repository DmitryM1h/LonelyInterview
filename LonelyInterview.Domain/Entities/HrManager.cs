


using LonelyInterview.Domain.Interfaces;

namespace LonelyInterview.Domain.Entities;

public class HrManager : IEntity
{
    public Guid Id { get; init; }

    public string? Company { get; init; }

    private List<Vacancy> _vacancies = new();
    public IReadOnlyCollection<Vacancy> Vacancies => _vacancies.AsReadOnly();

    public void CreateVacancy(Vacancy vacancy)
    {
        //TODO Validation

        _vacancies.Add(vacancy);
    }

    public static HrManager CreateHr(Guid id, string? company)
    {
        return new HrManager()
        {
            Id = id,
            Company = company
        };
    }


    private HrManager() { }
   

}
