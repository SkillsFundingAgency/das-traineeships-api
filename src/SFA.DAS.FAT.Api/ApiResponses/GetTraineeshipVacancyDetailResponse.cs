using SFA.DAS.FAT.Domain.Entities;
using System.Collections.Generic;

namespace SFA.DAS.FAT.Api.ApiResponses
{
    public class GetTraineeshipVacancyDetailResponse : GetTraineeshipVacancyResponse
    {
        public string LongDescription { get; set; }
        public string OutcomeDescription { get; set; }
        public string WorkExperience { get; set; }
        public List<string> Skills { get; set; }
        public string ThingsToConsider { get; set; }


        public static implicit operator GetTraineeshipVacancyDetailResponse(TraineeshipVacancyItem source)
        {
            return new GetTraineeshipVacancyDetailResponse
            {

                Id = source.Id,
                AnonymousEmployerName = source.AnonymousEmployerName,
                Category = source.Category,
                CategoryCode = source.CategoryCode,
                ClosingDate = source.ClosingDate,
                Description = source.Description,
                EmployerName = source.EmployerName,
                HoursPerWeek = source.HoursPerWeek,
                IsDisabilityConfident = source.IsDisabilityConfident,
                IsEmployerAnonymous = source.IsEmployerAnonymous,
                IsPositiveAboutDisability = source.IsPositiveAboutDisability,
                Location = source.Location,
                NumberOfPositions = source.NumberOfPositions,
                PostedDate = source.PostedDate,
                ProviderName = source.ProviderName,
                StartDate = source.StartDate,
                Title = source.Title,
                Ukprn = source.Ukprn,
                VacancyLocationType = (VacancyLocationType)source.VacancyLocationType,
                VacancyReference = source.VacancyReference,
                WorkingWeek = source.WorkingWeek,
                Distance = source.Distance,
                Score = source.Score,
                LongDescription = source.LongDescription,
                OutcomeDescription = source.OutcomeDescription,
                WorkExperience = source.WorkExperience,
                ThingsToConsider = source.ThingsToConsider,
                Skills = source.Skills,
                ExpectedDuration = !string.IsNullOrEmpty(source.ExpectedDuration)
                    ? source.ExpectedDuration
                    : $"{source.Duration} {(source.Duration == 1 || string.IsNullOrEmpty(source.DurationUnit) || source.DurationUnit.EndsWith("s") ? source.DurationUnit : $"{source.DurationUnit}s")}",
                EmployerContactName = source.EmployerContactName,
                EmployerContactEmail = source.EmployerContactEmail,
                EmployerContactPhone = source.EmployerContactPhone,
                EmployerWebsiteUrl = source.EmployerWebsiteUrl,
                EmployerDescription = source.EmployerDescription,
                Address = source.Address,
                RouteId = source.RouteId,
                RouteName = source.RouteName
            };
        }
    }
}