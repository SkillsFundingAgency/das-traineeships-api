using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.FAT.Api.ApiResponses;
using SFA.DAS.FAT.Api.ApRequests;
using SFA.DAS.FAT.Application.Vacancies.Queries.GetTraineeshipVacancy;
using SFA.DAS.FAT.Application.Vacancies.Queries.SearchTraineeshipVacancies;
using SFA.DAS.FAT.Domain.Models;
using System.Threading.Tasks;

namespace SFA.DAS.FAT.Api.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("/api/[controller]/")]
    public class VacanciesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public VacanciesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("{vacancyReference}")]
        public async Task<IActionResult> Get(string vacancyReference)
        {
            var result = await _mediator.Send(new GetTraineeshipVacancyQuery
            {
                VacancyReference = vacancyReference
            });

            if (result.TraineeshipVacancy == null)
            {
                return NotFound();
            }

            var apiResponse = (GetTraineeshipVacancyDetailResponse)result.TraineeshipVacancy;

            return Ok(apiResponse);
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Search([FromQuery] SearchVacancyRequest request)
        {
            var result = await _mediator.Send(new SearchTraineeshipVacanciesQuery
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                Ukprn = request.Ukprn,
                AccountPublicHashedId = request.AccountPublicHashedId,
                AccountLegalEntityPublicHashedId = request.AccountLegalEntityPublicHashedId,
                Lat = request.Lat,
                Lon = request.Lon,
                DistanceInMiles = request.DistanceInMiles,
                NationWideOnly = request.NationWideOnly,
                RouteIds = request.RouteIds,
                PostedInLastNumberOfDays = request.PostedInLastNumberOfDays,
                VacancySort = request.Sort ?? VacancySort.AgeDesc
            });

            var apiResponse = (GetSearchTraineeshipVacanciesResponse)result;

            return Ok(apiResponse);
        }
    }
}