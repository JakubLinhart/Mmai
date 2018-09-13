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

        [HttpGet]
        public async Task<JsonResult> Get()
        {
            var games = await repository.GetAll();

            return Json(games);
        }

        [HttpGet("csv")]
        public async Task<ActionResult> GetCsv()
        {
            var games = await repository.GetAll();

            return await this.Csv(games, "games.csv");
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Game value)
        {
            value.PlayerId = value.PlayerId ?? Request.Cookies["playerId"];
            var result = await repository.Insert(value);

            return Json(result);
        }
    }
}
