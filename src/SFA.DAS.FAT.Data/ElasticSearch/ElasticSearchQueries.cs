using SFA.DAS.FAT.Domain.Interfaces;
using System.IO;

namespace SFA.DAS.FAT.Data.ElasticSearch
{
    public class ElasticSearchQueries : IElasticSearchQueries
    {
        public string FindVacanciesQuery { get; }
        public string GetVacanciesCountQuery { get; }
        public string GetVacancyQuery { get; }

        public ElasticSearchQueries()
        {
            FindVacanciesQuery = File.ReadAllText("ElasticSearch/FindVacanciesQuery.json");
            GetVacanciesCountQuery = File.ReadAllText("ElasticSearch/GetVacanciesCountQuery.json");
            GetVacancyQuery = File.ReadAllText("ElasticSearch/GetVacancyQuery.json");
        }
    }
}
