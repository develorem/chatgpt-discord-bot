using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class HealthCheckController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        // Implement a simple health check here, return a 200 OK response if the application is running.
        return Ok();
    }
}

