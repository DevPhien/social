using Microsoft.AspNetCore.HttpOverrides;
using System;
using System.Net;
using System.Web;

namespace social.Controllers;

[ApiController]
[Route("api/facebook")]
[ApiExplorerSettings(GroupName = "Facebook")]
public class FacebookController : Controller
{
    private readonly ILogger<FacebookController> _logger;
    private readonly IFacebookHandler _facebookHandler;

    public FacebookController(ILogger<FacebookController> logger, IFacebookHandler facebookHandler)
    {
        _logger = logger;
        _facebookHandler = facebookHandler;
    }

    [HttpPost("addProfile")]
    public async Task<IActionResult> addProfile([FromBody] SocialResult socialResult)
    {
        var response = await _facebookHandler.handlerAddFacebookProfileAsync(socialResult.access_token, socialResult.refresh_token);
        return Ok(response);
    }

    [HttpDelete("deleteProfile")]
    public async Task<IActionResult> deleteProfile(string slug)
    {
        var response = await _facebookHandler.handlerDeleteFacebookProfileAsync(slug);
        return Ok(response);
    }

    //[HttpGet("getFacebookProfile")]
    //public async Task<IActionResult> getFacebookProfile(string slug)
    //{
    //    var response = await _facebookHandler.handlerGetFacebookProfileAsync(slug);
    //    return Ok(response);
    //}

    //[HttpGet("getFacebookProfiles")]
    //public async Task<IActionResult> getFacebookProfiles(string search, int pageNo, int pageSize)
    //{
    //    var response = await _facebookHandler.handlerGetFacebookProfilesAsync(search, pageNo, pageSize);
    //    return Ok(response);
    //}

    [HttpGet("getFacebookProfileInfo")]
    public async Task<IActionResult> getFacebookProfileInfo(string accessToken, string? refresh_token)
    {
        var response = await _facebookHandler.handlerGetProfileInfo(accessToken, refresh_token);
        return Ok(response);
    }

    [HttpGet("getUrlAccessToken")]
    public string getUrlAccessToken()
    {
        return _facebookHandler.handlerGetUrlAccessTokenAsync();
    }

    [HttpGet("callback")]
    public async Task<IActionResult> callback()
    {
        string fragment = HttpContext.Request.QueryString.ToUriComponent();
        var queryValues = QueryHelpers.ParseQuery(fragment);
        string code = queryValues?["code"].ToString() ?? "";
        var redirectUrl = await _facebookHandler.handlerGetProfileAccessTokenAsync(code);
        return Redirect(redirectUrl);
    }

    [HttpGet("getFacebookPagePosts")]
    public async Task<IActionResult> getFacebookPagePosts(string accessToken, int pageSize)
    {
        var response = await _facebookHandler.handlerGetFacebookPagePostsAsync(accessToken, pageSize);
        return Ok(response);
    }

    [HttpGet("getFacebookPosts")]
    public async Task<IActionResult> getFacebookPosts(string accessToken, int pageSize)
    {
        var response = await _facebookHandler.handlerGetFacebookPostsAsync(accessToken, pageSize);
        return Ok(response);
    }

    [HttpPost("postFacebookPublicPost")]
    public async Task<IActionResult> postFacebookPublicPost(string accessToken, string message, string? link, bool? isPublished)
    {
        var response = await _facebookHandler.handlerPublicPostAsync(accessToken, message, link, isPublished);
        return Ok(response);
    }

    [HttpPost("postLikeFacebookPost")]
    public async Task<IActionResult> postLikeFacebookPost(string accessToken, string feedId)
    {
        var response = await _facebookHandler.handlerLikeFacebookPostAsync(accessToken, feedId);
        return Ok(response);
    }

    [HttpPost("postCommentFacebookPost")]
    public async Task<IActionResult> postCommentFacebookPost(string accessToken, string feedId, string message)
    {
        var response = await _facebookHandler.handlerCommentFacebookPostAsync(accessToken, feedId, message);
        return Ok(response);
    }
}
