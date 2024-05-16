using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Api.ApiResponses;
using SFA.DAS.FAT.Domain.Entities;

namespace SFA.DAS.FAT.Api.UnitTests.ApiResponses
{
    public class WhenCastingToGetTraineeshipVacancyDetailResponse
    {
        [Test, AutoData]
        public void Then_The_Fields_Are_Mapped(TraineeshipVacancyItem source)
        {
            var actual = (GetTraineeshipVacancyDetailResponse)source;

            actual.Should().BeEquivalentTo(source, options => options
                .Excluding(c => c.Duration)
                .Excluding(c => c.DurationUnit)
            );
        }


        [Test]
        [InlineAutoData(1, "year", "1 year")]
        [InlineAutoData(3, "month", "3 months")]
        [InlineAutoData(3, "weeks", "3 weeks")]
        [InlineAutoData(3, null, "3 ")]
        public void Then_The_Expected_Duration_Is_Set_If_No_ExpectedDuration(int duration, string unit, string expectedText, TraineeshipVacancyItem source)
        {
            source.ExpectedDuration = null;
            source.Duration = duration;
            source.DurationUnit = unit;
            var response = (GetTraineeshipVacancyDetailResponse)source;

            response.ExpectedDuration.Should().Be(expectedText);
        }
    }
}