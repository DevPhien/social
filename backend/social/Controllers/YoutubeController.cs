namespace social.Controllers;

[ApiController]
[Route("api/youtube")]
[ApiExplorerSettings(GroupName = "youtube")]
public class YoutubeController : Controller
{
    private readonly ILogger<YoutubeController> _logger;
    private readonly IYoutubeHandler _youtubeHandler;

    public YoutubeController(ILogger<YoutubeController> logger, IYoutubeHandler youtubeHandler)
    {
        _logger = logger;
        _youtubeHandler = youtubeHandler;
    }

    [HttpPost("addProfile")]
    public async Task<object> addProfile([FromBody] SocialResult socialResult)
    {
        var response = await _youtubeHandler.handlerAddYoutubeProfileAsync(socialResult.access_token, socialResult.refresh_token);
        return Ok(response);
    }

    [HttpDelete("deleteProfile")]
    public async Task<IActionResult> deleteProfile(string slug)
    {
        var response = await _youtubeHandler.handlerDeleteYoutubeProfileAsync(slug);
        return Ok(response);
    }

    //[HttpGet("getYoutubekProfile")]
    //public async Task<IActionResult> getYoutubekProfile(string slug)
    //{
    //    var response = await _youtubeHandler.handlerGetYoutubeProfileAsync(slug);
    //    return Ok(response);
    //}

    //[HttpGet("getYoutubeProfiles")]
    //public async Task<IActionResult> getYoutubeProfiles(string search, int pageNo, int pageSize)
    //{
    //    var response = await _youtubeHandler.handlerGetYoutubeProfilesAsync(search, pageNo, pageSize);
    //    return Ok(response);
    //}

    [HttpGet("getUrlAccessToken")]
    public string getUrlAccessToken()
    {
        return _youtubeHandler.handlerGetUrlAccessTokenTokenAsync();
    }

    [HttpGet("callback")]
    public async Task<object> callback()
    {
        string fragment = HttpContext.Request.QueryString.ToUriComponent();
        var queryValues = QueryHelpers.ParseQuery(fragment);
        string code = queryValues?["code"].ToString() ?? "";
        var redirectUrl = await _youtubeHandler.handlerGetProfileAccessTokenAsync(code);
        return Redirect(redirectUrl);
    }

    [HttpGet("getProfileInfo")]
    public async Task<object> getProfileInfo(string accessToken, string refresh_token)
    {
        var response = await _youtubeHandler.handlerGetProfileInfo(accessToken, refresh_token);
        return Ok(response);
    }

    [HttpPost("uploadVideo")]
    public async Task<IActionResult> uploadVideo(string accessToken, string? refreshToken, string videoPath)
    {
        var response = await _youtubeHandler.handlerUploadVideo(accessToken, refreshToken, videoPath);
        return Ok(response);
    }

    [HttpPost("setThumbnail")]
    public async Task<IActionResult> setThumbnail(string accessToken, string videoId, string thumbnailPath)
    {
        var response = await _youtubeHandler.handlerUploadVideo(accessToken, videoId, thumbnailPath);
        return Ok(response);
    }

    [HttpGet("getInfo")]
    public async Task<IActionResult> getInfo(string accessToken)
    {
        var response = await _youtubeHandler.handlerGetInfo(accessToken);
        return Ok(response);
    }

    [HttpGet("getVideos")]
    public async Task<IActionResult> getVideos(string accessToken)
    {
        var response = await _youtubeHandler.handlerGetVideos(accessToken);
        return Ok(response);
    }

    [HttpPost("likeVideo")]
    public async Task<IActionResult> likeVideo(string accessToken, string videoId, string rating)
    {
        var response = await _youtubeHandler.handlerLikeVideo(accessToken, videoId, rating);
        return Ok(response);
    }

    [HttpPost("commentVideo")]
    public async Task<IActionResult> commentVideo(string accessToken, string videoId, string comment)
    {
        var response = await _youtubeHandler.handlerCommentVideo(accessToken, videoId, comment);
        return Ok(response);
    }
}

