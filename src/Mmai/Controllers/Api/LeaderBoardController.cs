using Microsoft.AspNetCore.Mvc;
using Mmai.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mmai.Controllers.Api
{
    [Route("api/leaderboard")]
    public class LeaderboardController : Controller
    {
        private readonly ILeaderboardRepository repository;

        public LeaderboardController(ILeaderboardRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet("{speciesName}")]
        public async Task<JsonResult> Get(string speciesName)
        {
            var leaderBoard = await repository.GetTopTen(speciesName);

            return Json(leaderBoard);
        }
    }
}
