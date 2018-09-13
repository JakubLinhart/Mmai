using Microsoft.AspNetCore.Mvc;
using Mmai.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mmai.Controllers.Api
{
    [Route("api/species")]
    public class SpeciesController : Controller
    {
        private readonly ISpeciesRepository repository;

        public SpeciesController(ISpeciesRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet("{name}")]
        public JsonResult Get(string name)
        {
            return Json(repository.Get(name));
        }
    }
}
