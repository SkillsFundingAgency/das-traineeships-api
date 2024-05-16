using SFA.DAS.FAT.Domain.Models;

namespace SFA.DAS.FAT.Domain.Interfaces
{
    public interface IElasticSearchQueryBuilder
    {
        string BuildFindVacanciesQuery(FindVacanciesModel findVacanciesModel);
        string BuildGetVacanciesCountQuery();
        string BuildGetVacancyQuery(string vacancyReference);
    }
}