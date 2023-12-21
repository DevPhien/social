using Microsoft.AspNetCore.HttpOverrides;

namespace social.Controllers;

[ApiController]
[Route("api/social")]
[ApiExplorerSettings(GroupName = "Social")]
public class SocialController : Controller
{
    private readonly ILogger<SocialController> _logger;
    private readonly ISocialHandler _socialHandler;

    public SocialController(ILogger<SocialController> logger, ISocialHandler socialHandler)
    {
        _logger = logger;
        _socialHandler = socialHandler;
    }

    [HttpGet("login")]
    public async Task<IActionResult> login(string network)
    {
        var reponse = _socialHandler.handlerLogin(network);
        return Ok(reponse);
    }

    [HttpGet("getProfiles")]
    public async Task<IActionResult> getProfiles()
    {
        var reponse = await _socialHandler.handlerGetProfiles();
        return Ok(reponse);
    }

    [HttpPost("publicPost")]
    public async Task<IActionResult> publicPost(IFormCollection form)
    {
        var reponse = await _socialHandler.handlerPublicPost(form);
        return Ok(reponse);
    }
}
