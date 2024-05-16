﻿using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Api.ApiResponses;
using SFA.DAS.FAT.Domain.Entities;

namespace SFA.DAS.FAT.Api.UnitTests.ApiResponses
{
    public class WhenCastingToGetTraineeshipVacancyResponse
    {
        [Test, AutoData]
        public void Then_Maps_Fields(TraineeshipSearchItem source)
        {
            source.ExpectedDuration = null;

            var response = (GetTraineeshipVacancyResponse)source;

            response.Should().BeEquivalentTo(source, options => options
                .Excluding(c => c.Duration)
                .Excluding(c => c.DurationUnit)
                .Excluding(c => c.EmployerDescription)
                .Excluding(c => c.ExpectedDuration)
            );
            response.ExpectedDuration.Should().Be($"{source.Duration} {(source.Duration == 1 ? source.DurationUnit : $"{source.DurationUnit}s")}");
        }

        [Test]
        [InlineAutoData(1, "year", "1 year")]
        [InlineAutoData(3, "month", "3 months")]
        [InlineAutoData(3, "weeks", "3 weeks")]
        [InlineAutoData(3, null, "3 ")]
        public void Then_The_Expected_Duration_Is_Set(int duration, string unit, string expectedText, TraineeshipSearchItem source)
        {
            source.ExpectedDuration = null;
            source.Duration = duration;
            source.DurationUnit = unit;

            var response = (GetTraineeshipVacancyResponse)source;

            response.ExpectedDuration.Should().Be(expectedText);
        }

        [Test, AutoData]
        public void Then_If_ExpectedDuration_Then_Used(TraineeshipSearchItem source)
        {
            var response = (GetTraineeshipVacancyResponse)source;

            response.Should().BeEquivalentTo(source, options => options
                .Excluding(c => c.Duration)
                .Excluding(c => c.DurationUnit)
                .Excluding(c => c.EmployerDescription)
            );
        }


        [Test, AutoData]
        public void Then_If_No_Address_Then_Null_Returned(TraineeshipVacancyItem source)
        {
            source.Address = null;

            var actual = (GetTraineeshipVacancyResponse)source;

            actual.Address.Should().BeNull();
        }
    }
}