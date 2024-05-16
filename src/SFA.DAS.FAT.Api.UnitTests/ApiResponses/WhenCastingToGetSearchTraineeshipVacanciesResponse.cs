using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Api.ApiResponses;
using SFA.DAS.FAT.Application.Vacancies.Queries.SearchTraineeshipVacancies;

namespace SFA.DAS.FAT.Api.UnitTests.ApiResponses
{
    public class WhenCastingToGetSearchTraineeshipVacanciesResponse
    {
        [Test, AutoData]
        public void Then_Maps_Fields(SearchTraineeshipVacanciesResult source)
        {
            var response = (GetSearchTraineeshipVacanciesResponse)source;

            response.Should().BeEquivalentTo(source, options => options.Excluding(c => c.TraineeshipVacancies));
            response.TraineeshipVacancies.Should().BeEquivalentTo(source.TraineeshipVacancies, options => options
                .Excluding(c => c.EmployerDescription)
                .Excluding(c => c.Duration)
                .Excluding(c => c.DurationUnit)
                .Excluding(c => c.ExpectedDuration)
            );
        }
    }
}