using Microsoft.Extensions.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using System;
using System.IO;

namespace SFA.DAS.FAT.Api.AppStart
{
    public static class SharedConfigurationBuilderExtension
    {
        public static IConfigurationRoot BuildSharedConfiguration(this IConfiguration configuration)
        {
            var config = new ConfigurationBuilder()
                .AddConfiguration(configuration)
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddEnvironmentVariables();

            if (!configuration["Environment"].Equals("DEV", StringComparison.CurrentCultureIgnoreCase))
            {

#if DEBUG
                config
                    .AddJsonFile("appsettings.json", true)
                    .AddJsonFile("appsettings.Development.json", true);
#endif

                config.AddAzureTableStorage(options =>
                {
                    options.ConfigurationKeys = configuration["ConfigNames"].Split(",");
                    options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
                    options.EnvironmentName = configuration["Environment"];
                    options.PreFixConfigurationKeys = false;
                }
                );
            }

            return config.Build();
        }

        public static bool IsLocalOrDev(this IConfiguration configuration)
        {
            return configuration.IsLocal() || configuration.IsDev();
        }
        public static bool IsDev(this IConfiguration configuration)
        {
            return configuration["Environment"].Equals("DEV", StringComparison.CurrentCultureIgnoreCase);
        }
        public static bool IsLocal(this IConfiguration configuration)
        {
            return configuration["Environment"].Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase);
        }
    }
}