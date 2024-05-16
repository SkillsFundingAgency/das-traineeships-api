using Elasticsearch.Net;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Data.Repository;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.FAT.Data.UnitTests.Repository
{
    public class WhenPingingElasticSearchServer
    {
        private const string IndexName = "test-faa-traineeships";
        private Mock<IElasticLowLevelClient> _mockClient;
        private ElasticEnvironment _apiEnvironment;
        private TraineeshipVacancySearchRepository _repository;

        [SetUp]
        public void Init()
        {
            _mockClient = new Mock<IElasticLowLevelClient>();
            _apiEnvironment = new ElasticEnvironment("test");
            _repository = new TraineeshipVacancySearchRepository(
                _mockClient.Object,
                _apiEnvironment,
                Mock.Of<IElasticSearchQueryBuilder>(),
                Mock.Of<ILogger<TraineeshipVacancySearchRepository>>());
        }

        [Test]
        public async Task Then_Returns_True_If_Api_Call_Successful()
        {
            //Arrange
            var apiCallMock = new Mock<IApiCallDetails>();
            apiCallMock
                .Setup(api => api.Success)
                .Returns(true);

            _mockClient
                .Setup(c => c.CountAsync<StringResponse>(IndexName, It.IsAny<PostData>(), It.IsAny<CountRequestParameters>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new StringResponse { ApiCall = apiCallMock.Object });

            //Act
            var result = await _repository.Ping();

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task Then_Returns_False_If_Api_Call_Fails()
        {
            //Arrange
            var apiCallMock = new Mock<IApiCallDetails>();
            apiCallMock
                .Setup(api => api.Success)
                .Returns(false);

            _mockClient
                .Setup(c => c.CountAsync<StringResponse>(IndexName, It.IsAny<PostData>(), It.IsAny<CountRequestParameters>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new StringResponse { ApiCall = apiCallMock.Object });

            //Act
            var result = await _repository.Ping();

            //Assert
            Assert.IsFalse(result);
        }
    }
}
