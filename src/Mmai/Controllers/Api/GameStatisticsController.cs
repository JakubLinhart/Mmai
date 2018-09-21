using Microsoft.AspNetCore.Mvc;
using Mmai.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mmai.Controllers.Api
{
    [Route("api/statistics")]
    public class GameStatisticsController : Controller
    {
        private readonly IGameStatisticsRepository repository;

        public GameStatisticsController(IGameStatisticsRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet("top10/{speciesId}")]
        public async Task<JsonResult> GetTop10(string speciesId)
        {
            var playerId = Request.Cookies["playerId"];

            return Json(await repository.GetTopTen(playerId, speciesId));
        }

        [HttpGet("game/{currentTurnCount}/{currentGameId}/{speciesId}")]
        public async Task<JsonResult> GetGameStatistics(int currentTurnCount, string currentGameId, string speciesId)
        {
            var playerId = Request.Cookies["playerId"];

            return Json(await repository.GetOneGameStatistics(currentTurnCount, currentGameId, playerId, speciesId));
        }

        [HttpGet("allgames/{speciesId}")]
        public async Task<JsonResult> GetAllGamesStatistics(string speciesId)
        {
            return Json(await repository.GetAllGamesStatistics(speciesId));
        }
    }
}
