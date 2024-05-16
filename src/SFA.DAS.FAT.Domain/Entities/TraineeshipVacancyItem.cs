using System.Collections.Generic;

namespace SFA.DAS.FAT.Domain.Entities
{
    public class TraineeshipVacancyItem : TraineeshipSearchItem
    {
        public string LongDescription { get; set; }
        public string OutcomeDescription { get; set; }
        public string WorkExperience { get; set; }
        public string ThingsToConsider { get; set; }
        public List<string> Skills { get; set; }
    }
}