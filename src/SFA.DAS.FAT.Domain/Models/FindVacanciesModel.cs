using System.Collections.Generic;

namespace SFA.DAS.FAT.Domain.Models
{
    public class FindVacanciesModel
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int? Ukprn { get; set; }
        public string AccountPublicHashedId { get; set; }
        public string AccountLegalEntityPublicHashedId { get; set; }
        public List<int> RouteId { get; set; }
        public List<string> Categories { get; set; }
        public double? Lat { get; set; }
        public double? Lon { get; set; }
        public uint? DistanceInMiles { get; set; }
        public bool? NationWideOnly { get; set; }
        public uint? PostedInLastNumberOfDays { get; set; }
        public VacancySort VacancySort { get; set; }
    }
}