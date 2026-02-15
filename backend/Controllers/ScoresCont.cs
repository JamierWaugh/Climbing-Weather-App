using Microsoft.AspNetCore.Mvc;
using Climbing_Weather_App.Weather;
using Climbing_Weather_App.Models;

[ApiController]
[Route("/scores")]
public class ScoresController : ControllerBase
{
    private readonly  Weather _weather;

    public ScoresController(Weather weather)
    {
        _weather = weather;
    }

    [HttpGet]
    public async Task<IActionResult> Scores([FromBody] ScoresUserRequest request)
    {
        try {
            await _weather.GetWeatherValues(request.rock_type, request.postcode);
            return Ok("Scores returned correctly");
        }
        /* TO DO
        Add better Exception and error control*/
        catch (Exception e)
        {
            return BadRequest($"Error with exception {e}");
        }
        
    }
}
