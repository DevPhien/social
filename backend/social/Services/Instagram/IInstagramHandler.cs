namespace social.Services;
public interface IInstagramHandler
{
    public Task<string> handlerGetProfileAccessTokenAsync(string code);
    public Task<object> handlerAddInstagramProfileAsync(string? access_token, string? refresh_token);
    public Task<object> handlerDeleteInstagramProfileAsync(string slug);
    public string handlerGetUrlAccessTokenTokenAsync();
    public Task<social_profile?> handlerGetProfileInfo(string? access_token, string? refresh_token);
    public Task<object> handlerGetInstagramPostsAsync(string access_token, int pageSize);
    public Task<object> handlerPublicPostAsync(string access_token, string message, string? link, bool? isPublished);
    public Task<object> handlerLikeInstagramPostAsync(string access_token, string postId);
    public Task<object> handlerCommentInstagramPostAsync(string access_token, string postId, string message);
}
