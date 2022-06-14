﻿using System.Threading.Tasks;
using SFA.DAS.FAT.Domain.Entities;
using SFA.DAS.FAT.Domain.Models;

namespace SFA.DAS.FAT.Domain.Interfaces
{
    public interface IVacancySearchRepository
    {
        Task<bool> Ping();
        Task<TraineeshipVacancyItem> Get(string vacancyReference);
        Task<TraineeshipSearchResponse> Find(FindVacanciesModel findVacanciesModel);
    }
}