using Microsoft.AspNetCore.Mvc;

namespace Nitroterm.Backend.Controllers;

[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
    [HttpGet]
    public string Get()
    {
        return "Hello, World !";
    }
}