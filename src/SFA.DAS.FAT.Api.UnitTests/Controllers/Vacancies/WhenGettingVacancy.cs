using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Api.ApiResponses;
using SFA.DAS.FAT.Api.Controllers;
using SFA.DAS.FAT.Application.Vacancies.Queries.GetTraineeshipVacancy;
using SFA.DAS.Testing.AutoFixture;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.FAT.Api.UnitTests.Controllers.Vacancies
{
    public class WhenGettingVacancy
    {
        [Test, MoqAutoData]
        public async Task Then_Gets_Result_From_Mediator(
            string vacancyReference,
            GetTraineeshipVacancyResult mediatorResult,
            [Frozen] Mock<IMediator> mockMediator,
            [Greedy] VacanciesController controller)
        {
            mockMediator
                .Setup(mediator => mediator.Send(
                    It.Is<GetTraineeshipVacancyQuery>(query =>
                        query.VacancyReference == vacancyReference),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(mediatorResult);

            var result = await controller.Get(vacancyReference) as OkObjectResult;

            result.Should().NotBeNull();
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var apiModel = result.Value as GetTraineeshipVacancyDetailResponse;
            apiModel.Should().NotBeNull();
            apiModel.Should().BeEquivalentTo((GetTraineeshipVacancyDetailResponse)mediatorResult.TraineeshipVacancy);
        }

        [Test, MoqAutoData]
        public async Task And_Null_From_Mediator_Then_Returns_NotFound(
            string vacancyReference,
            [Frozen] Mock<IMediator> mockMediator,
            [Greedy] VacanciesController controller)
        {
            mockMediator
                .Setup(mediator => mediator.Send(
                    It.Is<GetTraineeshipVacancyQuery>(query =>
                        query.VacancyReference == vacancyReference),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetTraineeshipVacancyResult());

            var result = await controller.Get(vacancyReference) as NotFoundResult;

            result.Should().NotBeNull();
            result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }
    }
}