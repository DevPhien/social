namespace social.Services;
public interface ISocialHandler
{
    public object handlerLogin(string network);
    public Task<object> handlerPublicPost(IFormCollection form);
    public Task<object> handlerGetProfiles();
}
