using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Mmai.Data;
using Mmai.Models;

namespace Mmai.Controllers.Api
{
    [Route("api/events")]
    public class GameEventController : Controller
    {
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]GameEventData value)
        {
            var playerId = Request.Cookies["playerId"];

            var gameId = value.GameId ?? Repository.NewGuid();
            var result = await Repository.AddGameEvent(playerId, gameId, value.Label, value.Card, value.Time, value.MillisecondsSinceLastEvent);

            return Json(gameId);
        }

        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
