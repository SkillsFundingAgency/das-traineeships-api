using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Vacancies.Queries.GetTraineeshipVacancy;
using SFA.DAS.FAT.Domain.Entities;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.FAT.Application.UnitTests.Vacancies.Queries
{
    public class WhenGettingTraineeshipVacancy
    {
        [Test, MoqAutoData]
        public async Task Then_Gets_Vacancy_From_Repository(
            GetTraineeshipVacancyQuery query,
            TraineeshipVacancyItem responseFromRepository,
            [Frozen] Mock<IVacancySearchRepository> mockVacancyIndexRepository,
            GetTraineeshipVacancyQueryHandler handler)
        {
            mockVacancyIndexRepository
                .Setup(repository => repository.Get(query.VacancyReference))
                .ReturnsAsync(responseFromRepository);

            var result = await handler.Handle(query, CancellationToken.None);

            result.TraineeshipVacancy
                .Should().BeEquivalentTo(responseFromRepository);
        }
    }
}