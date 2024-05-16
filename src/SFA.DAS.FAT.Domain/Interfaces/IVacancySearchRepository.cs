using SFA.DAS.FAT.Domain.Entities;
using SFA.DAS.FAT.Domain.Models;
using System.Threading.Tasks;

namespace SFA.DAS.FAT.Domain.Interfaces
{
    public interface IVacancySearchRepository
    {
        Task<bool> Ping();
        Task<TraineeshipVacancyItem> Get(string vacancyReference);
        Task<TraineeshipSearchResponse> Find(FindVacanciesModel findVacanciesModel);
    }
}
