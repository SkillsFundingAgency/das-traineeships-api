﻿using SFA.DAS.FAT.Data.Extensions;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Domain.Models;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.FAT.Data.ElasticSearch
{
    public class ElasticSearchQueryBuilder : IElasticSearchQueryBuilder
    {
        private readonly IElasticSearchQueries _elasticSearchQueries;

        public ElasticSearchQueryBuilder(IElasticSearchQueries elasticSearchQueries)
        {
            _elasticSearchQueries = elasticSearchQueries;
        }

        public string BuildFindVacanciesQuery(FindVacanciesModel findVacanciesModel)
        {
            var startingDocumentIndex = findVacanciesModel.PageNumber < 2 ? 0 : (findVacanciesModel.PageNumber - 1) * findVacanciesModel.PageSize;
            var mustConditions = BuildMustConditions(findVacanciesModel);
            var sort = BuildSort(findVacanciesModel);
            var filters = BuildFilters(findVacanciesModel);
            var parameters = new Dictionary<string, object>
            {
                {"pageSize", findVacanciesModel.PageSize},
                {nameof(startingDocumentIndex), startingDocumentIndex},
                {nameof(mustConditions), mustConditions},
                {nameof(sort), sort},
                {nameof(filters), filters}
            };

            var query = _elasticSearchQueries.FindVacanciesQuery.ReplaceParameters(parameters);

            return query;
        }

        public string BuildGetVacanciesCountQuery()
        {
            return _elasticSearchQueries.GetVacanciesCountQuery;
        }

        public string BuildGetVacancyQuery(string vacancyReference)
        {
            var parameters = new Dictionary<string, object>
            {
                {nameof(vacancyReference), vacancyReference}
            };
            return _elasticSearchQueries.GetVacancyQuery.ReplaceParameters(parameters);
        }

        private string BuildMustConditions(FindVacanciesModel findVacanciesModel)
        {
            var filters = string.Empty;
            if (findVacanciesModel.Ukprn.HasValue)
            {
                filters += @$"{{ ""term"": {{ ""ukprn"": ""{findVacanciesModel.Ukprn}"" }}}}";
            }
            if (!string.IsNullOrEmpty(findVacanciesModel.AccountPublicHashedId))
            {
                filters += @$"{AddFilterSeparator(filters)}{{ ""term"": {{ ""accountPublicHashedId"": ""{findVacanciesModel.AccountPublicHashedId}"" }}}}";
            }
            if (!string.IsNullOrEmpty(findVacanciesModel.AccountLegalEntityPublicHashedId))
            {
                filters += @$"{AddFilterSeparator(filters)}{{ ""term"": {{ ""accountLegalEntityPublicHashedId"": ""{findVacanciesModel.AccountLegalEntityPublicHashedId}"" }}}}";
            }
            if (findVacanciesModel.NationWideOnly.HasValue)
            {
                filters += @$"{AddFilterSeparator(filters)}{{ ""term"": {{ ""vacancyLocationType"": ""{(findVacanciesModel.NationWideOnly.Value ? "National" : "NonNational")}"" }}}}";
            }
            return filters;
        }



        private string BuildSort(FindVacanciesModel model)
        {
            switch (model.VacancySort)
            {
                case VacancySort.AgeAsc:
                    return @" { ""postedDate"" : { ""order"" : ""asc"" } }";
                case VacancySort.AgeDesc:
                    return @" { ""postedDate"" : { ""order"" : ""desc"" } }";
                case VacancySort.ExpectedStartDateAsc:
                    return @" { ""startDate"" : { ""order"" : ""asc"" } }";
                case VacancySort.ExpectedStartDateDesc:
                    return @" { ""startDate"" : { ""order"" : ""desc"" } }";
                case VacancySort.DistanceAsc:
                    return !model.Lat.HasValue || !model.Lon.HasValue ? "" : @$" {{ ""_geo_distance"" : {{ ""location"" : {{ ""lat"" : {model.Lat}, ""lon"" : {model.Lon} }}, ""order"" : ""asc"", ""unit"" :""mi"" }} }}";
                case VacancySort.DistanceDesc:
                    return !model.Lat.HasValue || !model.Lon.HasValue ? "" : @$" {{ ""_geo_distance"" : {{ ""location"" : {{ ""lat"" : {model.Lat}, ""lon"" : {model.Lon} }}, ""order"" : ""desc"", ""unit"" :""mi"" }} }}";
            }

            return "";
        }

        private string BuildFilters(FindVacanciesModel findVacanciesModel)
        {
            var filters = "";
            if (findVacanciesModel.Lat.HasValue && findVacanciesModel.Lon.HasValue && findVacanciesModel.DistanceInMiles.HasValue)
            {
                filters = $@"{{ ""geo_distance"": {{ ""distance"": ""{findVacanciesModel.DistanceInMiles}miles"", ""location"": {{ ""lat"": {findVacanciesModel.Lat}, ""lon"": {findVacanciesModel.Lon} }} }} }}";
            }

            if (findVacanciesModel.PostedInLastNumberOfDays.HasValue)
            {
                filters += @$"{AddFilterSeparator(filters)}{{ ""range"": {{ ""postedDate"": {{ ""gte"": ""now-{findVacanciesModel.PostedInLastNumberOfDays}d/d"", ""lt"": ""now/d"" }} }} }}";
            }

            if (findVacanciesModel.RouteIds != null && findVacanciesModel.RouteIds.Any())
            {
                filters += @$"{AddFilterSeparator(filters)}{{ ""terms"": {{ ""routeId"": [""{string.Join(@""",""", findVacanciesModel.RouteIds)}""] }} }}";
            }
            return filters;
        }
        private static string AddFilterSeparator(string filters)
        {
            if (!string.IsNullOrEmpty(filters))
            {
                return ", ";
            }

            return "";
        }
    }
}