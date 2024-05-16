using MediatR;

namespace SFA.DAS.FAT.Application.Vacancies.Queries.GetTraineeshipVacancy
{
    public class GetTraineeshipVacancyQuery : IRequest<GetTraineeshipVacancyResult>
    {
        public string VacancyReference { get; set; }
    }
}