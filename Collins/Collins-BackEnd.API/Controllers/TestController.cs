using Microsoft.AspNetCore.Mvc;

namespace Collins_BackEnd.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { Message = "Collins-BackEnd API is running successfully!", Time = DateTime.UtcNow });
    }

    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok("pong");
    }
}
