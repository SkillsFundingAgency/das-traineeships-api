using Elasticsearch.Net;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.FAT.Data.ElasticSearch;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Entities;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.FAT.Data.Repository
{
    public class TraineeshipVacancySearchRepository : IVacancySearchRepository
    {
        private readonly IElasticLowLevelClient _client;
        private readonly ElasticEnvironment _environment;
        private readonly IElasticSearchQueryBuilder _queryBuilder;
        private readonly ILogger<TraineeshipVacancySearchRepository> _logger;
        private const string IndexName = "-faa-traineeships";


        public TraineeshipVacancySearchRepository(
            IElasticLowLevelClient client,
            ElasticEnvironment environment,
            IElasticSearchQueryBuilder queryBuilder,
            ILogger<TraineeshipVacancySearchRepository> logger)
        {
            _client = client;
            _environment = environment;
            _queryBuilder = queryBuilder;
            _logger = logger;
        }

        private string TraineeshipVacanciesIndex => _environment.Prefix + IndexName;

        public async Task<bool> Ping()
        {
            var pingResponse = await _client.CountAsync<StringResponse>(
                TraineeshipVacanciesIndex,
                PostData.String(""),
                new CountRequestParameters(),
                CancellationToken.None);

            if (!pingResponse.Success)
            {
                _logger.LogDebug($"Elastic search ping failed: {pingResponse.DebugInformation ?? "no information available"}");
            }

            return pingResponse.Success;
        }

        public async Task<TraineeshipVacancyItem> Get(string vacancyReference)
        {
            _logger.LogInformation($"Starting get vacancy [{vacancyReference}]");

            var query = _queryBuilder.BuildGetVacancyQuery(vacancyReference);
            var jsonResponse = await _client.SearchAsync<StringResponse>(TraineeshipVacanciesIndex, PostData.String(query));
            var responseBody = JsonConvert.DeserializeObject<ElasticResponse<TraineeshipVacancyItem>>(jsonResponse.Body);

            _logger.LogInformation($"Found [{responseBody.hits.total.value}] hits for vacancy [{vacancyReference}]");

            var traineeshipVacancyItem = responseBody.Items.SingleOrDefault()?._source;
            return traineeshipVacancyItem;
        }

        public async Task<TraineeshipSearchResponse> Find(FindVacanciesModel findVacanciesModel)
        {
            _logger.LogInformation("Starting vacancy search");

            var query = _queryBuilder.BuildFindVacanciesQuery(findVacanciesModel);
            var jsonResponse = await _client.SearchAsync<StringResponse>(TraineeshipVacanciesIndex, PostData.String(query));
            var responseBody = JsonConvert.DeserializeObject<ElasticResponse<TraineeshipSearchItem>>(jsonResponse.Body);

            if (responseBody == null)
            {
                _logger.LogWarning("Searching failed. Elastic search response could not be de-serialised");
                return new TraineeshipSearchResponse();
            }

            _logger.LogDebug("Searching complete, returning search results");

            var totalRecordCount = await GetTotal();

            var traineeshipSearchItems = responseBody.Items;

            var searchItems = new List<TraineeshipSearchItem>();

            var isDistanceSort = findVacanciesModel.VacancySort == VacancySort.DistanceAsc ||
                                 findVacanciesModel.VacancySort == VacancySort.DistanceDesc;

            if (findVacanciesModel.Lat == null || findVacanciesModel.Lon == null
                                               || findVacanciesModel.DistanceInMiles == null || !isDistanceSort)
            {
                searchItems = traineeshipSearchItems.Select(c => c._source).ToList();
            }
            else
            {
                foreach (var traineeshipSearchItem in traineeshipSearchItems)
                {
                    var searchItem = traineeshipSearchItem._source;
                    searchItem.Distance = traineeshipSearchItem.sort?.FirstOrDefault();
                    searchItems.Add(searchItem);
                }
            }


            var searchResult = new TraineeshipSearchResponse
            {
                TraineeshipVacancies = searchItems,
                TotalFound = responseBody.hits?.total?.value ?? 0,
                Total = totalRecordCount
            };

            return searchResult;
        }

        private async Task<int> GetTotal()
        {
            var query = _queryBuilder.BuildGetVacanciesCountQuery();
            var jsonResponse = await _client.CountAsync<StringResponse>(TraineeshipVacanciesIndex, PostData.String(query));
            var result = JsonConvert.DeserializeObject<ElasticCountResponse>(jsonResponse.Body);

            return result.count;
        }
    }
}
