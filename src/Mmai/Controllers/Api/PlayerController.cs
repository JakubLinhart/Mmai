using Microsoft.AspNetCore.Mvc;
using Mmai.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mmai.Controllers.Api
{
    [Route("api/players")]
    public sealed class PlayerController : Controller
    {
        private readonly IPlayerRepository repository;

        public PlayerController(IPlayerRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet("all")]
        public async Task<JsonResult> GetAll()
        {
            var players = await repository.GetAll();

            return Json(players);
        }

        [HttpGet("all/csv")]
        public async Task<ActionResult> GetAllCsv()
        {
            var players = await repository.GetAll();

            return this.Csv(players, "players.csv");
        }

        [HttpGet("{playerId}")]
        public async Task<JsonResult> Get(string playerId)
        {
            var player = await repository.GetPlayer(playerId);

            return Json(player);
        }

        [HttpPost]
        public async Task<JsonResult> Update([FromBody]Player player)
        {
            var playerId = Request.Cookies["playerId"];

            if (string.IsNullOrEmpty(playerId))
                return Json(null);

            player = await repository.Update(playerId, player.NickName, player.Email);

            return Json(player);
        }

        public async Task<JsonResult> Get()
        {
            var playerId = Request.Cookies["playerId"];

            if (string.IsNullOrEmpty(playerId))
                return Json(null);

            var player = await repository.GetPlayer(playerId) ?? new Player
            {
                Id = playerId,
                NickName = "anonymous"
            };

            return Json(player);
        }
    }
}
