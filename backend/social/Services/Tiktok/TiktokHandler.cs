
using IdentityModel.Client;

namespace social.Services.Tiktok;

public class TiktokHandler : ITiktokHandler
{
    private readonly IConfiguration _configuration;
    private readonly SocialContext _context;
    public TiktokHandler(IConfiguration configuration, SocialContext context)
    {
        _configuration = configuration;
        _context = context;
    }

    public async Task<object> handlerAddTiktokProfileAsync(string? access_token, string? refresh_token)
    {
        var profile = await handlerGetProfileInfo(access_token, refresh_token);
        if (profile is not null)
        {
            try
            {
                bool exists = await _context.social_profiles.AnyAsync(x => x.id == profile.id);
                if (!exists)
                {
                    profile.key_id = Guid.NewGuid().ToString("N").ToUpper();
                    profile.profile_type = "tiktok";
                    profile.access_token = access_token;
                    profile.refresh_token = refresh_token;
                    _context.social_profiles.Add(profile);
                    await _context.SaveChangesAsync();
                }
                return new { error = false, message = "Successful", redirect_url = _configuration["Applications:Tiktok:redirect_url"] };
            }
            catch (DbUpdateException e)
            {
                return new { error = true, message = e.Message };
            }
            catch (Exception e)
            {
                return new { error = true, message = e.Message };
            }
        }
        else
        {
            return new { error = false, message = "Failed" };
        }
    }

    public async Task<object> handlerDeleteTiktokProfileAsync(string slug)
    {
        var profile = await _context.social_profiles.FindAsync(slug);
        if (profile is not null)
        {
            _context.social_profiles.Remove(profile);
            await _context.SaveChangesAsync();
            return new { error = false, message = "Successful" };
        }
        else
        {
            return new { error = false, message = "Failed" };
        }
    }

    public async Task<string> handlerGetProfileAccessTokenAsync(string code)
    {
        try
        {
            var client = new HttpClient();
            var tokenResponse = await client.RequestAuthorizationCodeTokenAsync(new AuthorizationCodeTokenRequest
            {
                Address = "https://open.tiktokapis.com/v2/oauth/token",
                ClientId = _configuration["Applications:Tiktok:client_key"],
                ClientSecret = _configuration["Applications:Tiktok:client_secret"],
                Code = code,
                RedirectUri = _configuration["Applications:Tiktok:redirect_add_profile_url"],
                GrantType = "authorization_code",
                Parameters =
                {
                    { "access_type", "offline" },
                    { "prompt", "consent" },
                    { "approval_prompt", "force" }
                }
            });
            return $"{_configuration["Applications:Tiktok:redirect_url"]}?profile_type=tiktok&access_token={tokenResponse.AccessToken}&refresh_token={tokenResponse.RefreshToken}";
        }
        catch
        {
            return $"{_configuration["Applications:Tiktok:redirect_url"]}";
        }
    }

    public async Task<social_profile?> handlerGetProfileInfo(string? access_token, string? refresh_token)
    {
        var queryParams = new Dictionary<string, string>
        {
            { "access_token", access_token ?? "" },
            { "refresh_token", refresh_token ?? "" },
            { "grant_type", "authorization_code" },
            { "fields", "open_id,union_id,avatar_url'" },
        };

        var _uriBuilder = new UriBuilder($"{_configuration["Applications:Tiktok:api"]}/{_configuration["Applications:Tiktok:version"]}/user/info");
        _uriBuilder.Query = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={kvp.Value}"));

        var _httpClient = new HttpClient();
        var response = await _httpClient.GetAsync(_uriBuilder.Uri);
        if (response.IsSuccessStatusCode)
        {
            return JsonConvert.DeserializeObject<social_profile>(await response.Content.ReadAsStringAsync());
        }
        else
        {
            return null;
        }
    }
}
