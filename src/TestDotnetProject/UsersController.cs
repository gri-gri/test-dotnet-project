using Microsoft.AspNetCore.Mvc;

namespace TestDotnetProject;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    [HttpPost]
    public IActionResult Create()
    {
        return Created();
    }
}
