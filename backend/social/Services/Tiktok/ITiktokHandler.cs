namespace social.Services;

public interface ITiktokHandler
{
    public Task<object> handlerAddTiktokProfileAsync(string? access_token, string? refresh_token);
    public Task<object> handlerDeleteTiktokProfileAsync(string slug);
    public Task<string> handlerGetProfileAccessTokenAsync(string code);
}
