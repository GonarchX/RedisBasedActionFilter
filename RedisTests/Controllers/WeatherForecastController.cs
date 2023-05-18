using Microsoft.AspNetCore.Mvc;

namespace RedisTests.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    [HttpGet(Name = "GetWeatherForecast")]
    [CachedActionFilter(5)]
    public async Task<ActionResult> Get(int id)
    {
        await Task.Delay(1000);
        return Ok(Summaries[id]);
    }
}