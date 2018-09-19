using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using Mmai.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Mmai.Controllers.Api
{
    [Route("api/games")]
    public class GameController : Controller
    {
        private readonly IGameRepository repository;

        public GameController(IGameRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet("all")]
        public async Task<JsonResult> GetAll()
        {
            var games = await repository.GetAll();

            return Json(games);
        }

        [HttpGet("all/csv")]
        public async Task<ActionResult> GetAllCsv()
        {
            var games = await repository.GetAll();

            return this.Csv(games, "games.csv");
        }

        [HttpPost("finish")]
        public async Task<IActionResult> Finish([FromBody]Game value)
        {
            value.PlayerId = value.PlayerId ?? Request.Cookies["playerId"];
            await repository.Update(value);

            return Json(value);
        }

        [HttpPost("start")]
        public async Task<JsonResult> Start([FromBody]Game value)
        {
            value.PlayerId = value.PlayerId ?? Request.Cookies["playerId"];
            var result = await repository.Insert(value);

            return Json(value);
        }
    }
}
