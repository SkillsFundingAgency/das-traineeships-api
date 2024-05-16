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
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.FAT.Data.UnitTests.Repository
{
    public class WhenGettingTraineeshipVacancy
    {
        private const string IndexName = "-faa-traineeships";

        [Test, MoqAutoData]
        public async Task And_Found_Then_Returns_Vacancy(
            string vacancyReference,
            [Frozen] ElasticEnvironment environment,
            [Frozen] Mock<IElasticLowLevelClient> mockElasticClient,
            TraineeshipVacancySearchRepository repository)
        {
            var expectedVacancy = JsonConvert
                .DeserializeObject<ElasticResponse<TraineeshipSearchItem>>(FakeElasticResponses.SingleHitResponse)
                .Items.First();

            mockElasticClient
                .Setup(client => client.SearchAsync<StringResponse>(
                    $"{environment.Prefix}{IndexName}",
                    It.IsAny<PostData>(),
                    It.IsAny<SearchRequestParameters>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new StringResponse(FakeElasticResponses.SingleHitResponse));

            var vacancy = await repository.Get(vacancyReference);

            vacancy.Should().BeEquivalentTo(expectedVacancy._source);
        }

        [Test, MoqAutoData]
        public async Task And_Not_Found_Then_Returns_Default(
            string vacancyReference,
            [Frozen] ElasticEnvironment environment,
            [Frozen] Mock<IElasticLowLevelClient> mockElasticClient,
            TraineeshipVacancySearchRepository repository)
        {
            mockElasticClient
                .Setup(client => client.SearchAsync<StringResponse>(
                    $"{environment.Prefix}{IndexName}",
                    It.IsAny<PostData>(),
                    It.IsAny<SearchRequestParameters>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new StringResponse(FakeElasticResponses.NoHitsResponse));

            var vacancy = await repository.Get(vacancyReference);

            vacancy.Should().BeNull();
        }

        [Test, MoqAutoData]
        public async Task And_More_Than_One_Hit_Then_Throws_Exception(
            string vacancyReference,
            [Frozen] ElasticEnvironment environment,
            [Frozen] Mock<IElasticLowLevelClient> mockElasticClient,
            TraineeshipVacancySearchRepository repository)
        {
            mockElasticClient
                .Setup(client => client.SearchAsync<StringResponse>(
                    $"{environment.Prefix}{IndexName}",
                    It.IsAny<PostData>(),
                    It.IsAny<SearchRequestParameters>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new StringResponse(FakeElasticResponses.MoreThanOneHitResponse));

            Func<Task> act = async () => await repository.Get(vacancyReference);

            await act.Should().ThrowAsync<InvalidOperationException>();
        }
    }
}