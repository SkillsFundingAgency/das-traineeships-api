using AutoFixture.NUnit3;
using Elasticsearch.Net;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.FAT.Data.ElasticSearch;
using SFA.DAS.FAT.Data.Repository;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Entities;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Domain.Models;
using SFA.DAS.Testing.AutoFixture;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.FAT.Data.UnitTests.Repository
{
    public class WhenFindingTraineeshipVacancies
    {
        private const string IndexName = "-faa-traineeships";

        [Test, MoqAutoData]
        public async Task Then_Will_Return_TraineeshipVacancies_Found_And_Distance_Null_If_No_Sort(
            FindVacanciesModel model,
            [Frozen] ElasticEnvironment environment,
            [Frozen] Mock<IElasticSearchQueryBuilder> mockQueryBuilder,
            [Frozen] Mock<IElasticLowLevelClient> mockElasticClient,
            TraineeshipVacancySearchRepository repository)
        {
            //arrange
            var expectedVacancy = JsonConvert
                .DeserializeObject<ElasticResponse<TraineeshipSearchItem>>(FakeElasticResponses.MoreThanOneHitResponse)
                .Items.First();

            mockElasticClient.Setup(c =>
                    c.SearchAsync<StringResponse>(
                        $"{environment.Prefix}{IndexName}",
                        It.IsAny<PostData>(),
                        It.IsAny<SearchRequestParameters>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(new StringResponse(FakeElasticResponses.MoreThanOneHitResponse));

            mockElasticClient.Setup(c =>
                    c.CountAsync<StringResponse>(
                        $"{environment.Prefix}{IndexName}",
                        It.IsAny<PostData>(),
                        It.IsAny<CountRequestParameters>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(new StringResponse(@"{""count"":10}"));

            //Act
            var results = await repository.Find(model);

            //Assert
            results.Total.Should().Be(10);
            results.TotalFound.Should().Be(2);
            results.TraineeshipVacancies.Count().Should().Be(2);
            var vacancy = results.TraineeshipVacancies.First();
            vacancy.Should().BeEquivalentTo(expectedVacancy._source);
            vacancy.Distance.Should().BeNull();
        }

        [Test, MoqAutoData]
        public async Task Then_Will_Return_TraineeshipVacancies_Found_And_Distance_If_Sort_And_GeoDistance(
            FindVacanciesModel model,
            [Frozen] ElasticEnvironment environment,
            [Frozen] Mock<IElasticSearchQueryBuilder> mockQueryBuilder,
            [Frozen] Mock<IElasticLowLevelClient> mockElasticClient,
            TraineeshipVacancySearchRepository repository)
        {
            //arrange
            model.VacancySort = VacancySort.DistanceAsc;
            var expectedVacancy = JsonConvert
                .DeserializeObject<ElasticResponse<TraineeshipSearchItem>>(FakeElasticResponses.MoreThanOneHitResponseWithSort)
                .Items.First();

            mockElasticClient.Setup(c =>
                    c.SearchAsync<StringResponse>(
                        $"{environment.Prefix}{IndexName}",
                        It.IsAny<PostData>(),
                        It.IsAny<SearchRequestParameters>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(new StringResponse(FakeElasticResponses.MoreThanOneHitResponseWithSort));

            mockElasticClient.Setup(c =>
                    c.CountAsync<StringResponse>(
                        $"{environment.Prefix}{IndexName}",
                        It.IsAny<PostData>(),
                        It.IsAny<CountRequestParameters>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(new StringResponse(@"{""count"":10}"));

            //Act
            var results = await repository.Find(model);

            //Assert
            results.Total.Should().Be(10);
            results.TotalFound.Should().Be(2);
            results.TraineeshipVacancies.Count().Should().Be(2);
            var vacancy = results.TraineeshipVacancies.First();
            vacancy.Should().BeEquivalentTo(expectedVacancy._source, options => options.Excluding(c => c.Distance));
            vacancy.Distance.Should().Be(expectedVacancy.sort.FirstOrDefault());
        }


        [Test]
        [MoqInlineAutoData(null, null, null)]
        [MoqInlineAutoData(1.0, null, null)]
        [MoqInlineAutoData(null, 1.0, null)]
        [MoqInlineAutoData(null, null, 1u)]
        [MoqInlineAutoData(1.0, 1.0, 1u, VacancySort.AgeAsc)]
        [MoqInlineAutoData(1.0, 1.0, 1u, VacancySort.AgeDesc)]
        [MoqInlineAutoData(1.0, 1.0, 1u, VacancySort.ExpectedStartDateAsc)]
        [MoqInlineAutoData(1.0, 1.0, 1u, VacancySort.ExpectedStartDateDesc)]
        public async Task Then_Will_Return_TraineeshipVacancies_Found_And_Distance_Null_If_Not_GeoDistance(
            double? lat,
            double? lon,
            uint? distanceInMiles,
            VacancySort vacancySort,
            FindVacanciesModel model,
            [Frozen] ElasticEnvironment environment,
            [Frozen] Mock<IElasticSearchQueryBuilder> mockQueryBuilder,
            [Frozen] Mock<IElasticLowLevelClient> mockElasticClient,
            TraineeshipVacancySearchRepository repository)
        {
            //arrange
            model.Lat = lat;
            model.Lon = lon;
            model.DistanceInMiles = distanceInMiles;
            model.VacancySort = vacancySort;
            var expectedVacancy = JsonConvert
                .DeserializeObject<ElasticResponse<TraineeshipSearchItem>>(FakeElasticResponses.MoreThanOneHitResponseWithSort)
                .Items.First();

            mockElasticClient.Setup(c =>
                    c.SearchAsync<StringResponse>(
                        $"{environment.Prefix}{IndexName}",
                        It.IsAny<PostData>(),
                        It.IsAny<SearchRequestParameters>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(new StringResponse(FakeElasticResponses.MoreThanOneHitResponseWithSort));

            mockElasticClient.Setup(c =>
                    c.CountAsync<StringResponse>(
                        $"{environment.Prefix}{IndexName}",
                        It.IsAny<PostData>(),
                        It.IsAny<CountRequestParameters>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(new StringResponse(@"{""count"":10}"));

            //Act
            var results = await repository.Find(model);

            //Assert
            results.Total.Should().Be(10);
            results.TotalFound.Should().Be(2);
            results.TraineeshipVacancies.Count().Should().Be(2);
            var vacancy = results.TraineeshipVacancies.First();
            vacancy.Should().BeEquivalentTo(expectedVacancy._source, options => options.Excluding(c => c.Distance));
            vacancy.Distance.Should().BeNull();
        }

        [Test, MoqAutoData]
        public async Task Then_Will_Return_Empty_Result_If_TraineeshipVacanciesIndex_Request_Returns_Invalid_Response(
            FindVacanciesModel model,
            [Frozen] ElasticEnvironment environment,
            [Frozen] Mock<IElasticSearchQueryBuilder> mockQueryBuilder,
            [Frozen] Mock<IElasticLowLevelClient> mockElasticClient,
            TraineeshipVacancySearchRepository repository)
        {
            //Arrange
            mockElasticClient.Setup(c =>
                    c.SearchAsync<StringResponse>(
                        $"{environment.Prefix}{IndexName}",
                        It.IsAny<PostData>(),
                        It.IsAny<SearchRequestParameters>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(new StringResponse(""));

            //Act
            var result = await repository.Find(model);

            //Assert
            Assert.IsNotNull(result?.TraineeshipVacancies);
            Assert.IsEmpty(result.TraineeshipVacancies);
            Assert.AreEqual(0, result.TotalFound);
        }

        [Test, MoqAutoData]
        public async Task Then_Will_Return_Empty_Result_If_TraineeshipVacanciesIndex_Request_Returns_No_results(
            FindVacanciesModel model,
            [Frozen] ElasticEnvironment environment,
            [Frozen] Mock<IElasticSearchQueryBuilder> mockQueryBuilder,
            [Frozen] Mock<IElasticLowLevelClient> mockElasticClient,
            TraineeshipVacancySearchRepository repository)
        {
            //Arrange
            var response = @"{""took"":0,""timed_out"":false,""_shards"":{""total"":1,""successful"":0,""skipped"":0,""failed"":1}}";

            mockElasticClient.Setup(c =>
                    c.SearchAsync<StringResponse>(
                        $"{environment.Prefix}{IndexName}",
                        It.IsAny<PostData>(),
                        It.IsAny<SearchRequestParameters>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(new StringResponse(response));

            mockElasticClient.Setup(c =>
                    c.CountAsync<StringResponse>(
                        $"{environment.Prefix}{IndexName}",
                        It.IsAny<PostData>(),
                        It.IsAny<CountRequestParameters>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(new StringResponse(@"{""count"":10}"));

            //Act
            var result = await repository.Find(model);

            //Assert
            Assert.IsNotNull(result?.TraineeshipVacancies);
            Assert.IsEmpty(result.TraineeshipVacancies);
            Assert.AreEqual(0, result.TotalFound);
        }

        [Test, MoqAutoData]
        public async Task Then_Will_Return_Empty_Result_If_TraineeshipVacanciesIndex_Request_Returns_Failed_Response(
            FindVacanciesModel model,
            [Frozen] ElasticEnvironment environment,
            [Frozen] Mock<IElasticSearchQueryBuilder> mockQueryBuilder,
            [Frozen] Mock<IElasticLowLevelClient> mockElasticClient,
            TraineeshipVacancySearchRepository repository)
        {
            //Arrange
            var response = @"{""took"":0,""timed_out"":false,""_shards"":{""total"":1,""successful"":0,""skipped"":0,""failed"":1},""hits"":{""total"":
            {""value"":0,""relation"":""eq""},""max_score"":null,""hits"":[]}}";

            mockElasticClient.Setup(c =>
                    c.SearchAsync<StringResponse>(
                        $"{environment.Prefix}{IndexName}",
                        It.IsAny<PostData>(),
                        It.IsAny<SearchRequestParameters>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(new StringResponse(response));

            mockElasticClient.Setup(c =>
                    c.CountAsync<StringResponse>(
                        $"{environment.Prefix}{IndexName}",
                        It.IsAny<PostData>(),
                        It.IsAny<CountRequestParameters>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(new StringResponse(@"{""count"":10}"));

            //Act
            var result = await repository.Find(model);

            //Assert
            Assert.IsNotNull(result?.TraineeshipVacancies);
            Assert.IsEmpty(result.TraineeshipVacancies);
            Assert.AreEqual(0, result.TotalFound);
        }
    }
}
