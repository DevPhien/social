using IdentityModel.Client;
using Microsoft.EntityFrameworkCore;

namespace social.Services.Instagram;
public class InstagramHandler : IInstagramHandler
{
    private readonly IConfiguration _configuration;
    private readonly SocialContext _context;
    public InstagramHandler(IConfiguration configuration, SocialContext context)
    {
        _configuration = configuration;
        _context = context;
    }

    public async Task<object> handlerDialogOauth()
    {
        var _uriBuilder = new UriBuilder($"https://www.instagram.com/v18.0/dialog/oauth?client_id={_configuration["Applications:Instagram:client_id"]}&redirect_ur=https://localhost:7118&scope=comma,separated,scopes&response_type=code");

        var _httpClient = new HttpClient();
        var response = await _httpClient.GetAsync(_uriBuilder.Uri);
        string responseBody = await response.Content.ReadAsStringAsync();
        JObject jsonResponse = JObject.Parse(responseBody);

        return new { jsonResponse };
    }

    public async Task<object> handlerAddInstagramProfileAsync(string? access_token, string? refresh_token)
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
                    profile.profile_type = "facebook";
                    profile.access_token = access_token;
                    profile.refresh_token = refresh_token;
                    _context.social_profiles.Add(profile);
                    await _context.SaveChangesAsync();
                }
                return new { error = false, message = "Successful", redirect_url = _configuration["Applications:Facebook:redirect_url"] };
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

    public async Task<object> handlerDeleteInstagramProfileAsync(string slug)
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

    public string handlerGetUrlAccessTokenTokenAsync()
    {
        var authUri = "https://www.instagram.com/v18.0/dialog/oauth";
        var clientId = _configuration["Applications:Instagram:client_id"];
        return $"{authUri}?client_id={clientId}&redirect_uri={_configuration["Applications:Instagram:redirect_url"]}&response_type=code";
    }

    public async Task<string> handlerGetProfileAccessTokenAsync(string code)
    {
        try
        {
            var client = new HttpClient();
            var tokenResponse = await client.RequestAuthorizationCodeTokenAsync(new AuthorizationCodeTokenRequest
            {
                Address = "https://graph.instagram.com/v18.0/oauth/access_token",
                ClientId = _configuration["Applications:Instagram:client_id"],
                ClientSecret = _configuration["Applications:Instagram:client_secret"],
                Code = code,
                RedirectUri = _configuration["Applications:Instagram:redirect_url"],
                GrantType = "authorization_code"
            });

            return tokenResponse.AccessToken ?? "";
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public async Task<social_profile?> handlerGetProfileInfo(string? access_token, string? refresh_token)
    {
        var queryParams = new Dictionary<string, string>
        {
            { "access_token", access_token ?? "" },
            { "refresh_token", refresh_token ?? "" },
            { "grant_type", "authorization_code" },
            { "fields", "id,name,email,birthday,first_name,last_name,friends" },
        };

        var _uriBuilder = new UriBuilder($"{_configuration["Applications:Instagram:api"]}/{_configuration["Applications:Instagram:version"]}/me");
        _uriBuilder.Query = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={kvp.Value}"));

        var _httpClient = new HttpClient();
        var response = await _httpClient.GetAsync(_uriBuilder.Uri);
        string responseBody = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
        {
            return JsonConvert.DeserializeObject<social_profile>(await response.Content.ReadAsStringAsync());
        }
        else
        {
            return null;
        }
    }

    public async Task<object> handlerGetInstagramPostsAsync(string access_token, int pageSize)
    {
        var queryParams = new Dictionary<string, string>
        {
            { "access_token", access_token },
            { "grant_type", "authorization_code" },
            { "fields", "link,privacy,message,source,attachments,created_time,description,reactions.summary(total_count),comments.summary(true),type,application" },
            { "limits", pageSize.ToString() },
        };

        var _uriBuilder = new UriBuilder($"{_configuration["Applications:Instagram:api"]}/{_configuration["Applications:Instagram:version"]}/{_configuration["Applications:Instagram:me_id"]}/feed");
        _uriBuilder.Query = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={kvp.Value}"));

        var _httpClient = new HttpClient();
        var response = await _httpClient.GetAsync(_uriBuilder.Uri);
        string responseBody = await response.Content.ReadAsStringAsync();
        JObject responseJson = JObject.Parse(responseBody);
        if (response.IsSuccessStatusCode)
        {
            string data = responseJson["data"]?.ToString() ?? "";
            return new { error = false, result = data };
        }
        else
        {
            string error = responseJson["error"]?.ToString() ?? "";
            return new { error = true, result = error };
        }
    }

    static long GetUnixTimestampOfFutureDate()
    {
        // Replace this with logic to calculate the Unix timestamp of a future date
        // Example: Convert.ToDateTime("2023-12-31T12:00:00Z").ToUnixTimestamp();
        return 0;
    }


    public async Task<object> handlerPublicPostAsync(string access_token, string message, string? link, bool? isPublished)
    {
        long scheduledPublishTime = GetUnixTimestampOfFutureDate();
        var queryParams = new Dictionary<string, string>
        {
            { "access_token", access_token },
            { "grant_type", "authorization_code" },
            { "message", message },
            { "link", link ?? "" },
            { "published", (isPublished ?? false).ToString().ToLower() },
            { "scheduled_publish_time", scheduledPublishTime.ToString() }
        };

        var _uriBuilder = new UriBuilder($"{_configuration["Applications:Instagram:api"]}/{_configuration["Applications:Instagram:version"]}/{_configuration["Applications:Instagram:page_id"]}/feed");
        //_uriBuilder.Query = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={kvp.Value}"));

        var _httpClient = new HttpClient();
        //_httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
        var response = await _httpClient.PostAsync(_uriBuilder.Uri, new FormUrlEncodedContent(queryParams));
        string responseBody = await response.Content.ReadAsStringAsync();
        JObject responseJson = JObject.Parse(responseBody);
        if (response.IsSuccessStatusCode)
        {
            return new { error = false };
        }
        else
        {
            string error = responseJson["error"]?.ToString() ?? "";
            return new { error = true, result = error };
        }
    }

    public async Task<object> handlerLikeInstagramPostAsync(string access_token, string postId)
    {
        var queryParams = new Dictionary<string, string>
        {
            { "access_token", access_token },
            { "grant_type", "authorization_code" },
            { "type", "like" },
        };

        var _uriBuilder = new UriBuilder($"{_configuration["Applications:Instagram:api"]}/{_configuration["Applications:Instagram:version"]}/{_configuration["Applications:Instagram:page_id"]}_{postId}/likes");

        var _httpClient = new HttpClient();
        var response = await _httpClient.PostAsync(_uriBuilder.Uri, new FormUrlEncodedContent(queryParams));
        string responseBody = await response.Content.ReadAsStringAsync();
        JObject responseJson = JObject.Parse(responseBody);
        if (response.IsSuccessStatusCode)
        {
            return new { error = false };
        }
        else
        {
            string error = responseJson["error"]?.ToString() ?? "";
            return new { error = true, result = error };
        }
    }

    public async Task<object> handlerCommentInstagramPostAsync(string access_token, string postId, string message)
    {
        var queryParams = new Dictionary<string, string>
        {
            { "access_token", access_token },
            { "grant_type", "client_credentials" },
            { "message", message },
        };

        var _uriBuilder = new UriBuilder($"{_configuration["Applications:Instagram:api"]}/{_configuration["Applications:Instagram:version"]}/{_configuration["Applications:Instagram:page_id"]}_{postId}/comments");

        var _httpClient = new HttpClient();
        var response = await _httpClient.PostAsync(_uriBuilder.Uri, new FormUrlEncodedContent(queryParams));
        string responseBody = await response.Content.ReadAsStringAsync();
        JObject responseJson = JObject.Parse(responseBody);
        if (response.IsSuccessStatusCode)
        {
            return new { error = false };
        }
        else
        {
            string error = responseJson["error"]?.ToString() ?? "";
            return new { error = true, result = error };
        }
    }
}
