namespace social.Controllers;

[ApiController]
[Route("api/instagram")]
[ApiExplorerSettings(GroupName = "Instagram")]
public class InstagramController : Controller
{
    private readonly ILogger<InstagramController> _logger;
    private readonly IInstagramHandler _instagramHandler;

    public InstagramController(ILogger<InstagramController> logger, IInstagramHandler instagramHandler)
    {
        _logger = logger;
        _instagramHandler = instagramHandler;
    }

    [HttpPost("addProfile")]
    public async Task<IActionResult> addProfile([FromBody] SocialResult socialResult)
    {
        var response = await _instagramHandler.handlerAddInstagramProfileAsync(socialResult.access_token, socialResult.refresh_token);
        return Ok(response);
    }

    [HttpGet("getUrlAccessToken")]
    public string getUrlAccessToken()
    {
        return _instagramHandler.handlerGetUrlAccessTokenTokenAsync();
    }

    [HttpGet("callback")]
    public async Task<string> callback()
    {
        string fragment = HttpContext.Request.QueryString.ToUriComponent();
        var queryValues = QueryHelpers.ParseQuery(fragment);
        string code = queryValues?["code"].ToString() ?? "";
        return await _instagramHandler.handlerGetProfileAccessTokenAsync(code);
    }

    [HttpGet("getInstagramPosts")]
    public async Task<IActionResult> getInstagramPosts(string access_token, int pageSize)
    {
        var response = await _instagramHandler.handlerGetInstagramPostsAsync(access_token, pageSize);
        return Ok(response);
    }

    [HttpPost("postInstagramPublicPost")]
    public async Task<IActionResult> postInstagramPublicPost(string access_token, string message, string? link, bool? isPublished)
    {
        var response = await _instagramHandler.handlerPublicPostAsync(access_token, message, link, isPublished);
        return Ok(response);
    }

    [HttpPost("postLikeInstagramPost")]
    public async Task<IActionResult> postLikeInstagramPost(string access_token, string postId)
    {
        var response = await _instagramHandler.handlerLikeInstagramPostAsync(access_token, postId);
        return Ok(response);
    }

    [HttpPost("postCommentInstagramPost")]
    public async Task<IActionResult> postCommentInstagramPost(string access_token, string postId, string message)
    {
        var response = await _instagramHandler.handlerCommentInstagramPostAsync(access_token, postId, message);
        return Ok(response);
    }
}
