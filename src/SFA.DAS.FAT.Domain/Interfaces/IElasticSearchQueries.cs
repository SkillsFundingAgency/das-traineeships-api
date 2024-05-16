namespace SFA.DAS.FAT.Domain.Interfaces
{
    public interface IElasticSearchQueries
    {
        string FindVacanciesQuery { get; }
        string GetVacanciesCountQuery { get; }
        string GetVacancyQuery { get; }
    }
}