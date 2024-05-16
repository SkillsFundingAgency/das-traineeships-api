using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Api.ApiResponses;
using SFA.DAS.FAT.Api.ApRequests;
using SFA.DAS.FAT.Api.Controllers;
using SFA.DAS.FAT.Application.Vacancies.Queries.SearchTraineeshipVacancies;
using SFA.DAS.FAT.Domain.Models;
using SFA.DAS.Testing.AutoFixture;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.FAT.Api.UnitTests.Controllers.Vacancies
{
    public class WhenGettingVacancySearch
    {
        [Test, MoqAutoData]
        public async Task Then_Gets_Search_Result_From_Mediator(
            SearchVacancyRequest request,
            SearchTraineeshipVacanciesResult mediatorResult,
            [Frozen] Mock<IMediator> mockMediator,
            [Greedy] VacanciesController controller)
        {
            request.Sort = VacancySort.DistanceDesc;
            mockMediator
                .Setup(mediator => mediator.Send(
                    It.Is<SearchTraineeshipVacanciesQuery>(query =>
                        query.PageNumber == request.PageNumber &&
                        query.PageSize == request.PageSize &&
                        query.Ukprn == request.Ukprn &&
                        query.AccountPublicHashedId == request.AccountPublicHashedId &&
                        query.AccountLegalEntityPublicHashedId == request.AccountLegalEntityPublicHashedId &&
                        query.RouteIds == request.RouteIds &&
                        query.NationWideOnly == request.NationWideOnly &&
                        query.Lat.Equals(request.Lat) &&
                        query.Lon.Equals(request.Lon) &&
                        query.DistanceInMiles == request.DistanceInMiles &&
                        query.PostedInLastNumberOfDays == request.PostedInLastNumberOfDays &&
                        query.VacancySort.Equals(request.Sort)
                    ),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(mediatorResult);

            var result = await controller.Search(request) as OkObjectResult;

            result.Should().NotBeNull();
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var apiModel = result.Value as GetSearchTraineeshipVacanciesResponse;
            apiModel.Should().NotBeNull();
            apiModel.Should().BeEquivalentTo((GetSearchTraineeshipVacanciesResponse)mediatorResult);
        }
    }
}