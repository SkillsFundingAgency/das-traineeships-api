using MediatR;
using SFA.DAS.FAT.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.FAT.Application.Vacancies.Queries.GetTraineeshipVacancy
{
    public class GetTraineeshipVacancyQueryHandler : IRequestHandler<GetTraineeshipVacancyQuery, GetTraineeshipVacancyResult>
    {
        private readonly IVacancySearchRepository _vacancySearchRepository;

        public GetTraineeshipVacancyQueryHandler(IVacancySearchRepository vacancySearchRepository)
        {
            _vacancySearchRepository = vacancySearchRepository;
        }

        public async Task<GetTraineeshipVacancyResult> Handle(GetTraineeshipVacancyQuery request, CancellationToken cancellationToken)
        {
            var vacancy = await _vacancySearchRepository.Get(request.VacancyReference);

            return new GetTraineeshipVacancyResult
            {
                TraineeshipVacancy = vacancy
            };
        }
    }
}