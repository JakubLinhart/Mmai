using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using Mmai.Models;

namespace Mmai.Controllers.Api
{
    [Route("api/events")]
    public class GameEventController : Controller
    {
        private readonly IGameEventRepository repository;

        public GameEventController(IGameEventRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet("all")]
        public async Task<JsonResult> GetAll()
        {
            var events = await repository.GetAll();

            return Json(events);
        }

        [HttpGet("all/csv")]
        public async Task<ActionResult> GetAllCsv()
        {
            var events = await repository.GetAll();

            return this.Csv(events, "events.csv");

        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]GameEvent value)
        {
            var playerId = Request.Cookies["playerId"];

            if (string.IsNullOrEmpty(value.GameId))
                value.GameId = Helpers.NewGuid();
            value.PlayerId = playerId;

            var result = await repository.Insert(value);

            return Json(value.GameId);
        }
    }
}
