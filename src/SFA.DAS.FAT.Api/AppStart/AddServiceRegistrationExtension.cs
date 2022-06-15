using Elasticsearch.Net;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.FAT.Data.ElasticSearch;
using SFA.DAS.FAT.Data.Repository;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Interfaces;
using System;

namespace SFA.DAS.FAT.Api.AppStart
{
    public static class AddServiceRegistrationExtension
    {
        public static void AddServiceRegistration(this IServiceCollection services)
        {
            services.AddTransient<IElasticSearchQueries, ElasticSearchQueries>();
            services.AddTransient<IElasticSearchQueryBuilder, ElasticSearchQueryBuilder>();
            services.AddTransient<IVacancySearchRepository, TraineeshipVacancySearchRepository>();
        }

        public static void AddElasticSearch(this IServiceCollection collection, FindTraineeshipsApiConfiguration configuration)
        {
            var connectionPool = new SingleNodeConnectionPool(new Uri(configuration.ElasticSearchServerUrl));

            var settings = new ConnectionConfiguration(connectionPool);

            if (!string.IsNullOrEmpty(configuration.ElasticSearchUsername) &&
                !string.IsNullOrEmpty(configuration.ElasticSearchPassword))
            {
                settings.BasicAuthentication(configuration.ElasticSearchUsername, configuration.ElasticSearchPassword);
            }

            collection.AddTransient<IElasticLowLevelClient>(sp => new ElasticLowLevelClient(settings));
            collection.AddSingleton<IElasticSearchQueries, ElasticSearchQueries>();
        }
    }
}
