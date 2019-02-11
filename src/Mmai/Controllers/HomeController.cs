using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mmai.Models;

namespace Mmai.Controllers
{
    public class HomeController : Controller
    {
        private readonly IGameEventRepository repository;

        public HomeController(IGameEventRepository repository)
        {
            this.repository = repository;
        }

        public IActionResult Index(string id="nextrandom")
        {
            var playerId = Request.Cookies["playerId"];

            if (string.IsNullOrEmpty(playerId))
            {
                var options = new CookieOptions();
                options.Expires = DateTime.Now.AddDays(14);
                Response.Cookies.Append("playerId", Helpers.NewGuid(), options);
            }

            ViewData["speciesId"] = id;

            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Results()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
