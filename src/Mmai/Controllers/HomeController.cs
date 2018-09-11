﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mmai.Data;
using Mmai.Models;

namespace Mmai.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var playerId = Request.Cookies["playerId"];

            if (string.IsNullOrEmpty(playerId))
            {
                var options = new CookieOptions();
                options.Expires = DateTime.Now.AddDays(14);
                Response.Cookies.Append("playerId", Repository.NewGuid(), options);
            }

            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
