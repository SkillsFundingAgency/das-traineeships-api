using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using NLog.Web;
using SFA.DAS.Api.Common.AppStart;
using SFA.DAS.Api.Common.Configuration;
using SFA.DAS.Api.Common.Infrastructure;
using SFA.DAS.FAT.Api.AppStart;
using SFA.DAS.FAT.Application.Vacancies.Queries.SearchTraineeshipVacancies;
using SFA.DAS.FAT.Domain.Configuration;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseNLog();

var _configuration = builder.Configuration.BuildSharedConfiguration();

builder.Services.AddOptions();
builder.Services.Configure<FindTraineeshipsApiConfiguration>(_configuration.GetSection("FindTraineeshipsApi"));
builder.Services.AddSingleton(cfg => cfg.GetService<IOptions<FindTraineeshipsApiConfiguration>>().Value);
builder.Services.Configure<AzureActiveDirectoryConfiguration>(_configuration.GetSection("AzureAd"));
builder.Services.AddSingleton(cfg => cfg.GetService<IOptions<AzureActiveDirectoryConfiguration>>().Value);

#if DEBUG
    builder.Services.AddSingleton(new ElasticEnvironment("pp"));
#else
    builder.Services.AddSingleton(new ElasticEnvironment(_configuration["ResourceEnvironmentName"]));
#endif

var apiConfig = _configuration
    .GetSection("FindTraineeshipsApi")
    .Get<FindTraineeshipsApiConfiguration>();

builder.Services.AddElasticSearch(apiConfig);

if (!_configuration.IsLocalOrDev())
{
    var azureAdConfiguration = _configuration
        .GetSection("AzureAd")
        .Get<AzureActiveDirectoryConfiguration>();

    var policies = new Dictionary<string, string>
                {
                    {PolicyNames.Default, "Default"}
                };

    builder.Services.AddAuthentication(azureAdConfiguration, policies);
}

if (_configuration["Environment"] != "DEV")
{
    builder.Services.AddHealthChecks();
}

builder.Services.AddMediatR(typeof(SearchTraineeshipVacanciesQuery).Assembly);
builder.Services.AddServiceRegistration();

builder.Services
    .AddMvc(o =>
    {
        if (!_configuration.IsLocalOrDev())
        {
            o.Conventions.Add(new AuthorizeControllerModelConvention(new List<string>()));
        }
        o.Conventions.Add(new ApiExplorerGroupPerVersionConvention());
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddApplicationInsightsTelemetry(_configuration["APPINSIGHTS_INSTRUMENTATIONKEY"]);

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "FindTraineeshipsApi", Version = "v1" });
    c.OperationFilter<SwaggerVersionHeaderFilter>();
});

builder.Services.AddApiVersioning(opt =>
{
    opt.ApiVersionReader = new HeaderApiVersionReader("X-Version");
});

builder.Services.AddLogging();

var app = builder.Build();
var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "FindTraineeshipsApi v1");
    c.RoutePrefix = string.Empty;
});

if (!app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.ConfigureExceptionHandler(loggerFactory.CreateLogger("Startup"));

app.UseAuthentication();

if (!_configuration["Environment"].Equals("DEV", StringComparison.CurrentCultureIgnoreCase))
{
    app.UseHealthChecks();
}

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "api/{controller=Vacancies}/{action=Get}/{id?}");
});

app.Run();
