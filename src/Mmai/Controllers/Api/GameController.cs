using Microsoft.AspNetCore.Mvc;
using Mmai.Models;
using System;
using System.Collections.Generic;
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

        [HttpPost("finish")]
        public async Task<IActionResult> Post([FromBody]Game value)
        {
            value.PlayerId = value.PlayerId ?? Request.Cookies["playerId"];
            var result = await repository.Insert(value);

            return Json(result);
        }
    }
}
