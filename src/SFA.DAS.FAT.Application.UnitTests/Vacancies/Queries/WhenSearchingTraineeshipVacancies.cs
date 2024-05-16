using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Vacancies.Queries.SearchTraineeshipVacancies;
using SFA.DAS.FAT.Domain.Entities;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Domain.Models;
using SFA.DAS.Testing.AutoFixture;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.FAT.Application.UnitTests.Vacancies.Queries
{
    public class WhenSearchingTraineeshipVacancies
    {
        [Test, MoqAutoData]
        public async Task Then_Gets_Vacancies_From_Repository(
            SearchTraineeshipVacanciesQuery query,
            TraineeshipSearchResponse responseFromRepository,
            [Frozen] Mock<IVacancySearchRepository> mockVacancyIndexRepository,
            SearchTraineeshipVacanciesQueryHandler handler)
        {
            mockVacancyIndexRepository
                .Setup(repository => repository.Find(It.Is<FindVacanciesModel>(c =>
                        c.PageNumber.Equals(query.PageNumber) &&
                        c.PageSize.Equals(query.PageSize) &&
                        c.Ukprn.Equals(query.Ukprn) &&
                        c.AccountPublicHashedId.Equals(query.AccountPublicHashedId) &&
                        c.AccountLegalEntityPublicHashedId.Equals(query.AccountLegalEntityPublicHashedId) &&
                        c.RouteIds.Equals(query.RouteIds) &&
                        c.Lat.Equals(query.Lat) &&
                        c.Lon.Equals(query.Lon) &&
                        c.DistanceInMiles.Equals(query.DistanceInMiles) &&
                        c.NationWideOnly.Equals(query.NationWideOnly) &&
                        c.PostedInLastNumberOfDays.Equals(query.PostedInLastNumberOfDays) &&
                        c.VacancySort.Equals(query.VacancySort)
                        )))
                .ReturnsAsync(responseFromRepository);

            var result = await handler.Handle(query, CancellationToken.None);

            result.TraineeshipVacancies
                .Should().BeEquivalentTo(responseFromRepository.TraineeshipVacancies);
            result.TotalFound.Should().Be(responseFromRepository.TotalFound);
            result.Total.Should().Be(responseFromRepository.Total);
        }
    }
}