using Microsoft.AspNetCore.Mvc;
using SaillingLoc.Data;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TestController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("ping")]
    public IActionResult Ping()
    {
        var usersCount = _context.Users.Count();
        return Ok($"Base connectée. Il y a {usersCount} utilisateur(s).");
    }
}
