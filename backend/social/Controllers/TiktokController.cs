namespace social.Controllers;

[ApiController]
[Route("api/tiktok")]
[ApiExplorerSettings(GroupName = "Tiktok")]
public class TiktokController : Controller
{
    private readonly ILogger<TiktokController> _logger;
    private readonly ITiktokHandler _tiktokHandler;

    public TiktokController(ILogger<TiktokController> logger, ITiktokHandler tiktokHandler)
    {
        _logger = logger;
        _tiktokHandler = tiktokHandler;
    }

    [HttpPost("addProfile")]
    public async Task<IActionResult> addProfile([FromBody] SocialResult socialResult)
    {
        var response = await _tiktokHandler.handlerAddTiktokProfileAsync(socialResult.access_token, socialResult.refresh_token);
        return Ok(response);
    }

    [HttpDelete("deleteProfile")]
    public async Task<IActionResult> deleteProfile(string slug)
    {
        var response = await _tiktokHandler.handlerDeleteTiktokProfileAsync(slug);
        return Ok(response);
    }

    [HttpGet("callback")]
    public async Task<IActionResult> callback()
    {
        string fragment = HttpContext.Request.QueryString.ToUriComponent();
        var queryValues = QueryHelpers.ParseQuery(fragment);
        string code = queryValues?["code"].ToString() ?? "";
        var redirectUrl = await _tiktokHandler.handlerGetProfileAccessTokenAsync(code);
        return Redirect(redirectUrl);
    }
}
