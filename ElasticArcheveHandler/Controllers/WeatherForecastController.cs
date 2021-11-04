using ElasticArchiveHandler;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticArcheveHandler.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IElasticArchiveService _elasticArchiveService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IElasticArchiveService elasticArchiveService )
        {
            _logger = logger;
            _elasticArchiveService = elasticArchiveService;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {

            _elasticArchiveService.SnapShot(to: DateTime.Now,from: DateTime.Now.AddDays(-12));
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
