using SFA.DAS.FAT.Application.Vacancies.Queries.SearchTraineeshipVacancies;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.FAT.Api.ApiResponses
{
    public class GetSearchTraineeshipVacanciesResponse
    {
        public int Total { get; set; }
        public int TotalFound { get; set; }
        public IEnumerable<GetTraineeshipVacancyResponse> TraineeshipVacancies { get; set; }

        public static implicit operator GetSearchTraineeshipVacanciesResponse(SearchTraineeshipVacanciesResult source)
        {
            return new GetSearchTraineeshipVacanciesResponse
            {
                TraineeshipVacancies = source.TraineeshipVacancies.Select(item => (GetTraineeshipVacancyResponse)item),
                TotalFound = source.TotalFound,
                Total = source.Total
            };
        }
    }
}