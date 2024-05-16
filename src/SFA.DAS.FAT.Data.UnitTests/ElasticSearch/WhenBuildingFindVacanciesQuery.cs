﻿using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Data.ElasticSearch;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Domain.Models;
using SFA.DAS.Testing.AutoFixture;
using System.Collections.Generic;

namespace SFA.DAS.FAT.Data.UnitTests.ElasticSearch
{
    public class WhenBuildingFindVacanciesQuery
    {
        [Test, MoqAutoData]
        public void Then_Calculates_StartDocumentIndex_And_Adds_To_Query(
            int pageNumber,
            int pageSize,
            [Frozen] Mock<IElasticSearchQueries> mockQueries,
            ElasticSearchQueryBuilder queryBuilder)
        {
            //arr
            var model = new FindVacanciesModel
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            mockQueries
                .Setup(queries => queries.FindVacanciesQuery)
                .Returns(@"{""from"": ""{startingDocumentIndex}""}");
            var expectedStartingDocumentIndex = pageNumber < 2 ? 0 : (pageNumber - 1) * pageSize;

            //act
            var query = queryBuilder.BuildFindVacanciesQuery(model);

            //ass
            query.Should().Contain(@$"""from"": ""{expectedStartingDocumentIndex}""");
        }

        [Test, MoqAutoData]
        public void Then_Adds_PageSize_To_Query(
            int pageNumber,
            int pageSize,
            [Frozen] Mock<IElasticSearchQueries> mockQueries,
            ElasticSearchQueryBuilder queryBuilder)
        {
            //arr
            var model = new FindVacanciesModel
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            mockQueries
                .Setup(queries => queries.FindVacanciesQuery)
                .Returns(@"{""size"": ""{pageSize}""}");

            //act
            var query = queryBuilder.BuildFindVacanciesQuery(model);

            //ass
            query.Should().Contain(@$"""size"": ""{pageSize}""");
        }

        [Test]
        [MoqInlineAutoData(50000112, null, null, null, null, @"{""must"": [ { ""term"": { ""ukprn"": ""50000112"" }} ]")]
        [MoqInlineAutoData(null, "ACB123", null, null, null, @"{""must"": [ { ""term"": { ""accountPublicHashedId"": ""ACB123"" }} ]")]
        [MoqInlineAutoData(null, null, "XYZ456", null, null, @"{""must"": [ { ""term"": { ""accountLegalEntityPublicHashedId"": ""XYZ456"" }} ]")]
        [MoqInlineAutoData(null, null, null, null, true, @"{""must"": [ { ""term"": { ""vacancyLocationType"": ""National"" }} ]")]
        [MoqInlineAutoData(null, null, null, null, false, @"{""must"": [ { ""term"": { ""vacancyLocationType"": ""NonNational"" }} ]")]
        public void And_Single_Field_HasValue_Then_Adds_Must_Condition(
            int? ukprn,
            string accountPublicHashedId,
            string accountLegalEntityPublicHashedId,
            List<int> routeId,
            bool? national,
            string fieldAssertion,
            int pageNumber,
            int pageSize,
            [Frozen] Mock<IElasticSearchQueries> mockQueries,
            ElasticSearchQueryBuilder queryBuilder)
        {
            //arr
            var model = new FindVacanciesModel
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Ukprn = ukprn,
                AccountPublicHashedId = accountPublicHashedId,
                AccountLegalEntityPublicHashedId = accountLegalEntityPublicHashedId,
                RouteIds = routeId,
                NationWideOnly = national
            };
            mockQueries
                .Setup(queries => queries.FindVacanciesQuery)
                .Returns(@"{""must"": [ {mustConditions} ] }");

            //act
            var query = queryBuilder.BuildFindVacanciesQuery(model);

            //ass
            query.Should().Contain(fieldAssertion);
        }

        [Test, MoqAutoData]
        public void And_Ukprn_And_AccountId_And_AccountLegalEntity_HasValue_Then_Adds_Must_Condition(
            int pageNumber,
            int pageSize,
            int ukprn,
            string accountPublicHashedId,
            string accountLegalEntityPublicHashedId,
            [Frozen] Mock<IElasticSearchQueries> mockQueries,
            ElasticSearchQueryBuilder queryBuilder)
        {
            //arr
            var model = new FindVacanciesModel
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Ukprn = ukprn,
                AccountPublicHashedId = accountPublicHashedId,
                AccountLegalEntityPublicHashedId = accountLegalEntityPublicHashedId
            };
            mockQueries
                .Setup(queries => queries.FindVacanciesQuery)
                .Returns(@"{""must"": [ {mustConditions} ] }");

            //act
            var query = queryBuilder.BuildFindVacanciesQuery(model);

            //ass
            query.Should().Contain(@$"""must"": [ {{ ""term"": {{ ""ukprn"": ""{ukprn}"" }}}}, {{ ""term"": {{ ""accountPublicHashedId"": ""{accountPublicHashedId}"" }}}}, {{ ""term"": {{ ""accountLegalEntityPublicHashedId"": ""{accountLegalEntityPublicHashedId}"" }}}} ]");
        }

        [Test]
        [MoqInlineAutoData(VacancySort.AgeAsc, @" { ""postedDate"" : { ""order"" : ""asc"" } }")]
        [MoqInlineAutoData(VacancySort.AgeDesc, @" { ""postedDate"" : { ""order"" : ""desc"" } }")]
        [MoqInlineAutoData(VacancySort.DistanceAsc, @" { ""_geo_distance"" : { ""location"" : { ""lat"" : 1.0546, ""lon"" : -1.546 }, ""order"" : ""asc"", ""unit"" :""mi"" } }")]
        [MoqInlineAutoData(VacancySort.DistanceDesc, @" { ""_geo_distance"" : { ""location"" : { ""lat"" : 1.0546, ""lon"" : -1.546 }, ""order"" : ""desc"", ""unit"" :""mi"" } }")]
        [MoqInlineAutoData(VacancySort.ExpectedStartDateAsc, @" { ""startDate"" : { ""order"" : ""asc"" } }")]
        [MoqInlineAutoData(VacancySort.ExpectedStartDateDesc, @" { ""startDate"" : { ""order"" : ""desc"" } }")]
        public void And_Adds_Sort_And_Direction(
            VacancySort sort,
            string expectedSort,
            FindVacanciesModel model,
            [Frozen] Mock<IElasticSearchQueries> mockQueries,
            ElasticSearchQueryBuilder queryBuilder)
        {
            //Arrange
            model.Lat = 1.0546;
            model.Lon = -1.546;
            model.DistanceInMiles = null;
            model.VacancySort = sort;
            mockQueries
                .Setup(queries => queries.FindVacanciesQuery)
                .Returns(@"{""sort"": [ {sort} ] }");

            //Act
            var query = queryBuilder.BuildFindVacanciesQuery(model);

            //Assert
            query.Should().Contain(expectedSort);
        }

        [Test, MoqAutoData]
        public void Then_If_Location_And_Distance_Filter_Added(
            FindVacanciesModel model,
            [Frozen] Mock<IElasticSearchQueries> mockQueries,
            ElasticSearchQueryBuilder queryBuilder)
        {
            mockQueries
                .Setup(queries => queries.FindVacanciesQuery)
                .Returns(@"{filters}");

            //Act
            var query = queryBuilder.BuildFindVacanciesQuery(model);

            //Assert
            query.Should().Contain($@"{{ ""geo_distance"": {{ ""distance"": ""{model.DistanceInMiles}miles"", ""location"": {{ ""lat"": {model.Lat}, ""lon"": {model.Lon} }} }} }}");
        }

        [Test]
        [MoqInlineAutoData(null, 1.55, 3u)]
        [MoqInlineAutoData(1.55, null, 3u)]
        [MoqInlineAutoData(null, null, null)]
        public void Then_If_No_Location_Information_Then_Distance_Filter_Or_Sort_Not_Added(
            double? lat,
            double? lon,
            uint? distanceInMiles,
            FindVacanciesModel model,
            [Frozen] Mock<IElasticSearchQueries> mockQueries,
            ElasticSearchQueryBuilder queryBuilder)
        {
            //Arrange
            model.Lat = lat;
            model.Lon = lon;
            model.DistanceInMiles = distanceInMiles;
            model.PostedInLastNumberOfDays = null;
            model.RouteIds = null;
            model.VacancySort = VacancySort.DistanceAsc;
            mockQueries
                .Setup(queries => queries.FindVacanciesQuery)
                .Returns(@"{""sort"": [ {sort} ] } {filters}");

            //Act
            var query = queryBuilder.BuildFindVacanciesQuery(model);

            //Assert
            query.Should().Be(@"{""sort"": [  ] } ");
        }

        [Test, MoqAutoData]
        public void Then_If_There_Is_A_PostedInLastNumberOfDays_Value_Then_Added_To_Query(
            FindVacanciesModel model,
            [Frozen] Mock<IElasticSearchQueries> mockQueries,
            ElasticSearchQueryBuilder queryBuilder)
        {
            //Arrange
            model.Lat = null;
            model.RouteIds = null;
            mockQueries
                .Setup(queries => queries.FindVacanciesQuery)
                .Returns(@"{filters}");

            //Act

            var query = queryBuilder.BuildFindVacanciesQuery(model);

            //Assert
            query.Should().Be(@$"{{ ""range"": {{ ""postedDate"": {{ ""gte"": ""now-{model.PostedInLastNumberOfDays}d/d"", ""lt"": ""now/d"" }} }} }}");
        }

        [Test, MoqAutoData]
        public void Then_If_There_Is_A_PostedInLastNumberOfDays_And_Location_Then_Added_To_Query(
            FindVacanciesModel model,
            [Frozen] Mock<IElasticSearchQueries> mockQueries,
            ElasticSearchQueryBuilder queryBuilder)
        {
            //Arrange
            model.RouteIds = null;
            mockQueries
                .Setup(queries => queries.FindVacanciesQuery)
                .Returns(@"{filters}");

            //Act

            var query = queryBuilder.BuildFindVacanciesQuery(model);

            //Assert
            query.Should().Be(@$"{{ ""geo_distance"": {{ ""distance"": ""{model.DistanceInMiles}miles"", ""location"": {{ ""lat"": {model.Lat}, ""lon"": {model.Lon} }} }} }}, {{ ""range"": {{ ""postedDate"": {{ ""gte"": ""now-{model.PostedInLastNumberOfDays}d/d"", ""lt"": ""now/d"" }} }} }}");
        }

        [Test, MoqAutoData]
        public void Then_If_There_Are_Standard_Lars_Codes_They_Are_Added_To_The_Filter(
            FindVacanciesModel model,
            [Frozen] Mock<IElasticSearchQueries> mockQueries,
            ElasticSearchQueryBuilder queryBuilder)
        {
            //Arrange
            model.Lat = null;
            model.PostedInLastNumberOfDays = null;
            mockQueries
                .Setup(queries => queries.FindVacanciesQuery)
                .Returns(@"{filters}");

            //Act

            var query = queryBuilder.BuildFindVacanciesQuery(model);

            //Assert
            query.Should().Be(@$"{{ ""terms"": {{ ""routeId"": [""{string.Join(@""",""", model.RouteIds)}""] }} }}");
        }

        [Test, MoqAutoData]
        public void Then_If_There_Is_No_Value_For_PostedInLastNumberOfDays_And_Location_Value_Then_Not_Added_To_Filter_Query(
            FindVacanciesModel model,
            [Frozen] Mock<IElasticSearchQueries> mockQueries,
            ElasticSearchQueryBuilder queryBuilder)
        {
            //Arrange
            model.PostedInLastNumberOfDays = null;
            model.Lat = null;
            model.Lon = null;
            model.DistanceInMiles = null;
            model.RouteIds = null;
            mockQueries
                .Setup(queries => queries.FindVacanciesQuery)
                .Returns(@"{filters}");

            //Act
            var query = queryBuilder.BuildFindVacanciesQuery(model);

            //Assert
            query.Should().Be("");
        }
    }
}