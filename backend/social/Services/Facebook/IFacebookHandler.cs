namespace social.Services;

public interface IFacebookHandler
{
    public Task<string> handlerGetProfileAccessTokenAsync(string code);
    public Task<object> handlerAddFacebookProfileAsync(string? access_token, string? refresh_token);
    public Task<object> handlerDeleteFacebookProfileAsync(string slug);
    //public Task<SocialProfile?> handlerGetFacebookProfileAsync(string slug);
    //public Task<List<SocialProfile>?> handlerGetFacebookProfilesAsync(string search, int pageNo, int pageSize);
    public string handlerGetUrlAccessTokenAsync();
    public Task<social_profile?> handlerGetProfileInfo(string? access_token, string? refresh_token);
    public Task<object> handlerGetFacebookPagePostsAsync(string access_token, int pageSize);
    public Task<object> handlerGetFacebookPostsAsync(string access_token, int pageSize);
    public Task<object> handlerPublicPostAsync(string access_token, string message, string? link, bool? isPublished);
    public Task<object> handlerLikeFacebookPostAsync(string access_token, string feed_id);
    public Task<object> handlerCommentFacebookPostAsync(string access_token, string feed_id, string message);
}