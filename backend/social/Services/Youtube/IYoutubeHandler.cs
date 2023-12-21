namespace social.Services;

public interface IYoutubeHandler
{
    public Task<object> handlerAddYoutubeProfileAsync(string? access_token, string? refresh_token);
    public Task<object> handlerDeleteYoutubeProfileAsync(string slug);
    //public Task<SocialProfile?> handlerGetYoutubeProfileAsync(string slug);
    //public Task<List<SocialProfile>?> handlerGetYoutubeProfilesAsync(string search, int pageNo, int pageSize);
    public string handlerGetUrlAccessTokenTokenAsync();
    public Task<string> handlerGetProfileAccessTokenAsync(string code);
    public Task<social_profile?> handlerGetProfileInfo(string? access_token, string? refresh_token);
    public Task<object> handlerUploadVideo(string access_token, string? refreshToken, string videoPath);
    public Task<object> handlerSetThumbnail(string access_token, string videoId, string thumbnailPath);
    public Task<object> handlerGetInfo(string access_token);
    public Task<object> handlerGetVideos(string access_token);
    public Task<object> handlerLikeVideo(string access_token, string videoId, string rating);
    public Task<object> handlerCommentVideo(string access_token, string videoId, string comment);
}

