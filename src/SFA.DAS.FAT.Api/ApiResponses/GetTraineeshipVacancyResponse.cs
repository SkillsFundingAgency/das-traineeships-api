using SFA.DAS.FAT.Domain.Entities;
using System;

namespace SFA.DAS.FAT.Api.ApiResponses
{
    public class GetTraineeshipVacancyResponse
    {
        public int Id { get; set; }
        public string AnonymousEmployerName { get; set; }
        public DateTime ClosingDate { get; set; }
        public string Description { get; set; }
        public string EmployerName { get; set; }
        public decimal? HoursPerWeek { get; set; }
        public bool IsDisabilityConfident { get; set; }
        public bool IsEmployerAnonymous { get; set; }
        public bool IsPositiveAboutDisability { get; set; }
        public GeoPoint Location { get; set; }
        public int NumberOfPositions { get; set; }
        public DateTime PostedDate { get; set; }
        public string ProviderName { get; set; }
        public string Title { get; set; }
        public long Ukprn { get; set; }
        public VacancyLocationType VacancyLocationType { get; set; }
        public string VacancyReference { get; set; }
        public string WorkingWeek { get; set; }
        public string ExpectedDuration { get; set; }

        //Calculated after search
        public decimal? Distance { get; set; }
        public double Score { get; set; }
        public Address Address { get; set; }
        public string EmployerDescription { get; set; }
        public string EmployerWebsiteUrl { get; set; }
        public string EmployerContactPhone { get; set; }
        public string EmployerContactEmail { get; set; }
        public string EmployerContactName { get; set; }
        public int? RouteId { get; set; }
        public string RouteName { get; set; }
        public string Category { get; set; }
        public string CategoryCode { get; set; }
        public DateTime StartDate { get; set; }

        public static implicit operator GetTraineeshipVacancyResponse(TraineeshipSearchItem source)
        {
            return new GetTraineeshipVacancyResponse
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
                ExpectedDuration = !string.IsNullOrEmpty(source.ExpectedDuration)
                    ? source.ExpectedDuration
                    : $"{source.Duration} {(source.Duration == 1 || string.IsNullOrEmpty(source.DurationUnit) || source.DurationUnit.EndsWith("s") ? source.DurationUnit : $"{source.DurationUnit}s")}",
                EmployerContactName = source.EmployerContactName,
                EmployerContactEmail = source.EmployerContactEmail,
                EmployerContactPhone = source.EmployerContactPhone,
                EmployerWebsiteUrl = source.EmployerWebsiteUrl,
                Address = source.Address,
                RouteId = source.RouteId,
                RouteName = source.RouteName

            };
        }
    }

    public class GeoPoint
    {
        public double lon { get; set; }
        public double lat { get; set; }

        public static implicit operator GeoPoint(SFA.DAS.FAT.Domain.Entities.GeoPoint source)
        {
            return new GeoPoint
            {
                lon = source.lon,
                lat = source.lat
            };
        }
    }

    public class Address
    {
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressLine4 { get; set; }
        public string Postcode { get; set; }

        public static implicit operator Address(SFA.DAS.FAT.Domain.Entities.Address source)
        {
            if (source == null)
            {
                return null;
            }
            return new Address
            {
                AddressLine1 = source.AddressLine1,
                AddressLine2 = source.AddressLine2,
                AddressLine3 = source.AddressLine3,
                AddressLine4 = source.AddressLine4,
                Postcode = source.Postcode
            };
        }
    }
    public enum VacancyLocationType
    {
        Unknown = 0,
        NonNational,
        National
    }
}