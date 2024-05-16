using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Data.ElasticSearch;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Data.UnitTests.ElasticSearch
{
    public class WhenBuildingGetVacanciesCountQuery
    {
        [Test, MoqAutoData]
        public void Then_Returns_Query_From_Queries(
            [Frozen] Mock<IElasticSearchQueries> mockQueries,
            ElasticSearchQueryBuilder queryBuilder)
        {
            //act
            var query = queryBuilder.BuildGetVacanciesCountQuery();

            //ass
            query.Should().Be(mockQueries.Object.GetVacanciesCountQuery);
        }
    }
}