using SFA.DAS.FAT.Domain.Entities;
using System.Collections.Generic;

namespace SFA.DAS.FAT.Application.Vacancies.Queries.SearchTraineeshipVacancies
{
    public class SearchTraineeshipVacanciesResult
    {
        public IEnumerable<TraineeshipSearchItem> TraineeshipVacancies { get; set; }
        public int TotalFound { get; set; }
        public int Total { get; set; }
    }
}