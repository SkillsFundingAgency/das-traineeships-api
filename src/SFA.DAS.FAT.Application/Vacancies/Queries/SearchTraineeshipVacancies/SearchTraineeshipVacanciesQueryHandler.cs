using MediatR;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Domain.Models;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.FAT.Application.Vacancies.Queries.SearchTraineeshipVacancies
{
    public class SearchTraineeshipVacanciesQueryHandler : IRequestHandler<SearchTraineeshipVacanciesQuery, SearchTraineeshipVacanciesResult>
    {
        private readonly IVacancySearchRepository _vacancySearchRepository;

        public SearchTraineeshipVacanciesQueryHandler(IVacancySearchRepository vacancySearchRepository)
        {
            _vacancySearchRepository = vacancySearchRepository;
        }

        public async Task<SearchTraineeshipVacanciesResult> Handle(SearchTraineeshipVacanciesQuery request, CancellationToken cancellationToken)
        {
            var searchResult = await _vacancySearchRepository.Find(new FindVacanciesModel
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                Ukprn = request.Ukprn,
                AccountPublicHashedId = request.AccountPublicHashedId,
                AccountLegalEntityPublicHashedId = request.AccountLegalEntityPublicHashedId,
                RouteIds = request.RouteIds,
                Lat = request.Lat,
                Lon = request.Lon,
                DistanceInMiles = request.DistanceInMiles,
                NationWideOnly = request.NationWideOnly,
                PostedInLastNumberOfDays = request.PostedInLastNumberOfDays,
                VacancySort = request.VacancySort
            });

            return new SearchTraineeshipVacanciesResult
            {
                TraineeshipVacancies = searchResult.TraineeshipVacancies,
                TotalFound = searchResult.TotalFound,
                Total = searchResult.Total
            };
        }
    }
}