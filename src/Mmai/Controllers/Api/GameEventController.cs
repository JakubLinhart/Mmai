﻿using System;
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

        [HttpGet]
        public async Task<JsonResult> Get()
        {
            var events = await repository.GetAll();

            return Json(events);
        }

        [HttpGet("csv")]
        public async Task<ActionResult> GetCsv()
        {
            var events = await repository.GetAll();
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            var csvWriter = new CsvWriter(writer);

            csvWriter.WriteRecords<GameEvent>(events);
            await csvWriter.FlushAsync();

            return File(stream.GetBuffer(), "application/CSV", "events.csv");
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]GameEvent value)
        {
            var playerId = Request.Cookies["playerId"];

            if (string.IsNullOrEmpty(value.GameId))
                value.GameId = repository.NewGuid();
            value.PlayerId = playerId;

            var result = await repository.Insert(value);

            return Json(value.GameId);
        }
    }
}
