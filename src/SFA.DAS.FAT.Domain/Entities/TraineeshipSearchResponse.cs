using System.Collections.Generic;

namespace SFA.DAS.FAT.Domain.Entities
{
    public class TraineeshipSearchResponse
    {
        public IEnumerable<TraineeshipSearchItem> TraineeshipVacancies { get; set; } = new List<TraineeshipSearchItem>();
        public int TotalFound { get; set; }
        public int Total { get; set; }
    }
}
