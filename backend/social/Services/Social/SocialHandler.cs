

using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3.Data;
using Google.Apis.YouTube.v3;
using System.Reflection;
using System.Text;
using System.Threading;
using Tweetinvi.Core.Events;
using Tweetinvi.Core.Models;
using Tweetinvi.Models;
using static Google.Apis.Requests.BatchRequest;

namespace social.Services.Social;
public class SocialHandler : ISocialHandler
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;
    private readonly SocialContext _context;

    public SocialHandler(IConfiguration configuration, IWebHostEnvironment environment, SocialContext context)
    {
        _configuration = configuration;
        _environment = environment;
        _context = context;
    }

    public object handlerLogin(string network)
    {
        string redirectUrl = "";
        switch (network)
        {
            case "facebook":
                string fb_redirect_url = "https://localhost:7118/api/facebook/callback";
                string fb_client_id = "893904665525179";
                //string fb_login_scopes = "email,user_posts,user_birthday,publish_pages,manage_pages,publish_to_groups,user_friends";
                string fb_login_scopes = "email";
                redirectUrl = $"https://www.facebook.com/v18.0/dialog/oauth?response_type=code&redirect_uri={fb_redirect_url}&client_id={fb_client_id}&scope={fb_login_scopes}";
                break;
            case "youtube":
                // refresh_token = 1//0e1JnJ5PKLTC4CgYIARAAGA4SNwF-L9Ir7cRm0Y0mEeex2qVn8iQleKr1iZxTQiQgXuwezKXDeTKtlBDs6HDJ30fSpMOWXC7kGNE

                string yt_redirect_url = "https://localhost:7118/api/youtube/callback";
                string yt_client_id = "23219178779-r88htl4bkh6mj43v4nk9ul890dp9qro4.apps.googleusercontent.com";
                string yt_login_scopes = "https%3A%2F%2Fwww.googleapis.com%2Fauth%2Fuserinfo.email+https%3A%2F%2Fwww.googleapis.com%2Fauth%2Fuserinfo.profile";
                redirectUrl = $"https://accounts.google.com/o/oauth2/v2/auth?redirect_uri={yt_redirect_url}&prompt=consent&response_type=code&client_id={yt_client_id}&scope={yt_login_scopes}&access_type=offline";
                break;
            case "instagram":
                string ig_redirect_url = "https://localhost:7118/api/instagram/callback";
                string ig_client_id = "893904665525179";
                //string fb_login_scopes = "email,user_posts,user_birthday,publish_pages,manage_pages,publish_to_groups,user_friends";
                string ig_login_scopes = "email";
                redirectUrl = $"https://www.instagram.com/v18.0/dialog/oauth?response_type=code&redirect_uri={ig_redirect_url}&client_id={ig_client_id}&scope={ig_login_scopes}";
                break;
            case "twitter":

                break;
        }
        return new { error = false, result = redirectUrl };
    }

    //public async Task<object> handlerDeleteProfileAsync(string slug)
    //{
    //    var profile = await _context.SocialProfiles.FindAsync(slug);
    //    if (profile is not null)
    //    {
    //        _context.SocialProfiles.Remove(profile);
    //        await _context.SaveChangesAsync();
    //        return new { error = false, message = "Successful" };
    //    }
    //    else
    //    {
    //        return new { error = false, message = "Failed" };
    //    }
    //}

    //public async Task<SocialProfile?> handlerGetProfileAsync(string slug)
    //{
    //    var profile = await _context.SocialProfiles.FindAsync(slug);
    //    if (profile != null)
    //    {
    //        return profile;
    //    }
    //    else
    //    {
    //        return null;
    //    }
    //}

    //public async Task<List<SocialProfile>?> handlerGetProfilesAsync(string search, int pageNo, int pageSize)
    //{
    //    var profiles = await _context.SocialProfiles.Where(x => && (x.name ?? "").Contains(search)).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();
    //    if (profiles is not null && profiles.Count > 0)
    //    {
    //        return profiles;
    //    }
    //    else
    //    {
    //        return null;
    //    }
    //}

    public async Task<object> handlerPublicPost(IFormCollection form)
    {
        try
        {
            if (form is not null)
            {
                string socialForm = form["socialForm"].ToString();
                string socialProfiles = form["socialProfiles"].ToString();

                SocialForm? _socialForm = JsonConvert.DeserializeObject<SocialForm>(socialForm);
                List<social_profile>? _socialProfiles = JsonConvert.DeserializeObject<List<social_profile>>(socialProfiles);
                if (_socialForm is not null && _socialProfiles is not null && _socialProfiles.Count > 0)
                {
                    foreach (var socialProfile in _socialProfiles)
                    {
                        if (!string.IsNullOrEmpty(socialProfile.access_token))
                        {
                            if (socialProfile.profile_type == "facebook")
                            {
                                object? postData = null;

                                if (string.IsNullOrEmpty(_socialForm.post_type))
                                {
                                    _socialForm.post_type = "Text";
                                }
                                if (_socialForm.post_type == "Text")
                                {
                                    postData = new { message = _socialForm.message ?? "" };
                                }
                                else if (_socialForm.post_type == "OldImage")
                                {
                                    if (form.Files is not null)
                                    {
                                        var files = form.Files.Where(x => x.Name == "image").ToList();
                                        foreach (var file in files)
                                        {
                                            postData = new { message = _socialForm.message ?? "", Source = file };
                                            if (postData is not null)
                                            {
                                                var content = new StringContent(JsonConvert.SerializeObject(postData), Encoding.UTF8, "application/json");
                                                var _uriBuilder = new UriBuilder($"{_configuration["Applications:Facebook:api"]}/{_configuration["Applications:Facebook:version"]}/{socialProfile.page_id}/photos?access_token={socialProfile.access_token}");

                                                var _httpClient = new HttpClient();
                                                var response = await _httpClient.PostAsync(_uriBuilder.Uri, content);
                                            }
                                        }
                                    }
                                }
                                else if (_socialForm.post_type == "Image")
                                {
                                    List<string> mediaIds = new List<string>();
                                    if (form.Files is not null)
                                    {
                                        var files = form.Files.Where(x => x.Name == "image").ToList();
                                        foreach (var file in files)
                                        {
                                            var postDataDetail = new { 
                                                message = $"{_socialForm.message ?? ""} \n{_socialForm.link ?? ""}",
                                                Source = file,
                                                published = "false",
                                            };
                                            if (postDataDetail is not null)
                                            {
                                                var content = new StringContent(JsonConvert.SerializeObject(postDataDetail), Encoding.UTF8, "application/json");
                                                var _uriBuilder = new UriBuilder($"{_configuration["Applications:Facebook:api"]}/{_configuration["Applications:Facebook:version"]}/{socialProfile.page_id}/photos?access_token={socialProfile.access_token}");

                                                var _httpClient = new HttpClient();
                                                var response = await _httpClient.PostAsync(_uriBuilder.Uri, content);
                                                if (response.IsSuccessStatusCode)
                                                {
                                                    string responseBody = await response.Content.ReadAsStringAsync();
                                                    JObject responseJson = JObject.Parse(responseBody);
                                                    string mediaId = responseJson["id"]?.ToString() ?? "";
                                                    if(!string.IsNullOrEmpty(mediaId)){
                                                        mediaIds.Add(mediaId);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    List<string> attachmentDetails = new List<string>();
                                    if (mediaIds is not null && mediaIds.Count > 0)
                                    {
                                        for(int i = 0; i < mediaIds.Count; i++)
                                        {
                                            attachmentDetails.Add("{\"media_fbid\":" + $"\"{mediaIds[i]}\"" + "}");
                                        }
                                    }
                                    postData = new
                                    {
                                        message = $"{_socialForm.message ?? ""} \n{_socialForm.link ?? ""}",
                                        attached_media = attachmentDetails,
                                    };
                                }
                                else if (_socialForm.post_type == "Link")
                                {
                                    postData = new { message = _socialForm.message ?? "", link = _socialForm.link ?? "" };
                                }
                                else if (_socialForm.post_type == "Video")
                                {
                                    if (form.Files is not null && form.Files.Count > 0)
                                    {
                                        var files = form.Files.Where(x => x.Name == "image").ToList();
                                        if (files is not null && files.Count > 0)
                                        {
                                            var file = files[0];
                                            postData = new { description = $"{_socialForm.message ?? ""} \n{_socialForm.link ?? ""}", Source = file };
                                        }
                                    }
                                    if (postData is not null)
                                    {
                                        var content = new StringContent(JsonConvert.SerializeObject(postData), Encoding.UTF8, "application/json");
                                        var _uriBuilder = new UriBuilder($"{_configuration["Applications:Facebook:api"]}/{_configuration["Applications:Facebook:version"]}/{socialProfile.page_id}/videos?access_token={socialProfile.access_token}");

                                        var _httpClient = new HttpClient();
                                        var response = await _httpClient.PostAsync(_uriBuilder.Uri, content);
                                        if (response.IsSuccessStatusCode)
                                        {
                                            return new { error = false, message = "Successful" };
                                        }
                                        else
                                        {
                                            return new { error = false, message = response.Content.ReadAsStringAsync() };
                                        }
                                    }
                                }

                                if (postData is not null && _socialForm.post_type != "OldImage" && _socialForm.post_type != "Video")
                                {
                                    var content = new StringContent(JsonConvert.SerializeObject(postData), Encoding.UTF8, "application/json");
                                    var _uriBuilder = new UriBuilder($"{_configuration["Applications:Facebook:api"]}/{_configuration["Applications:Facebook:version"]}/{socialProfile.page_id}/feed?access_token={socialProfile.access_token}");

                                    var _httpClient = new HttpClient();
                                    var response = await _httpClient.PostAsync(_uriBuilder.Uri, content);
                                    if (response.IsSuccessStatusCode)
                                    {
                                        return new { error = false, message = "Successful" };
                                    }
                                    else
                                    {
                                        return new { error = false, message = response.Content.ReadAsStringAsync() };
                                    }
                                }
                            }
                            else if (socialProfile.profile_type == "youtube")
                            {
                                string root = _environment.WebRootPath;

                                var token = new Google.Apis.Auth.OAuth2.Responses.TokenResponse { AccessToken = socialProfile.access_token, RefreshToken = socialProfile.refresh_token };
                                var initializer = new GoogleAuthorizationCodeFlow.Initializer
                                {
                                    ClientSecrets = new ClientSecrets
                                    {
                                        ClientId = _configuration["Applications:Youtube:client_id"],
                                        ClientSecret = _configuration["Applications:Youtube:client_secret"],
                                    },
                                    DataStore = new FileDataStore("Store"),
                                    Scopes = new[] { YouTubeService.Scope.Youtube, YouTubeService.Scope.YoutubeUpload },
                                };
                                UserCredential credential = new UserCredential(new GoogleAuthorizationCodeFlow(initializer), "social-media", token);

                                var youtubeService = new YouTubeService(new BaseClientService.Initializer()
                                {
                                    // ApiKey = _configuration["Applications:Youtube:api_key"],
                                    HttpClientInitializer = credential,
                                    ApplicationName = Assembly.GetExecutingAssembly().GetName().Name
                                });

                                var video = new Video();
                                video.Snippet = new VideoSnippet();
                                video.Snippet.Title = _socialForm.message;
                                video.Snippet.Description = _socialForm.description;
                                video.Snippet.Tags = _socialForm.tags?.Split(",");
                                // video.Snippet.CategoryId = "22"; // See https://developers.google.com/youtube/v3/docs/videoCategories/list
                                video.Status = new VideoStatus();
                                string? published = _socialForm.published?.ToString().ToLower();
                                if (published == "false" || published == "private")
                                {
                                    published = "private";
                                }
                                else
                                {
                                    published = "public";
                                }
                                video.Status.PrivacyStatus = published; // or "private" or "public"

                                
                                if (form.Files is not null && form.Files.Count > 0)
                                {
                                    var files = form.Files.Where(x => x.Name == "video").ToList();
                                    if (files is not null && files.Count > 0)
                                    {
                                        var file = files[0];
                                        MemoryStream ms = new MemoryStream();
                                        await file.CopyToAsync(ms);
                                        ms.Seek(0, SeekOrigin.Begin);

                                        var videosInsertRequest = youtubeService.Videos.Insert(video, "snippet,status", ms, "video/*");
                                        // videosInsertRequest.ProgressChanged += videosInsertRequest_ProgressChanged;
                                        // videosInsertRequest.ResponseReceived += videosInsertRequest_ResponseReceived;

                                        var response = await videosInsertRequest.UploadAsync();
                                    }
                                }
                            }
                            else if (socialProfile.profile_type == "instagram")
                            {

                            }
                            else if (socialProfile.profile_type == "tiktok")
                            {
                                object? postData = null;

                                if (form.Files is not null && form.Files.Count > 0)
                                {
                                    var files = form.Files.Where(x => x.Name == "video").ToList();
                                    if (files is not null && files.Count > 0)
                                    {
                                        var file = files[0];
                                        postData = new { video = file, caption = $"{_socialForm.message ?? ""} \n{_socialForm.description ?? ""}" };
                                    }
                                }

                                if (postData is not null)
                                {
                                    var content = new StringContent(JsonConvert.SerializeObject(postData), Encoding.UTF8, "application/json");
                                    var _uriBuilder = new UriBuilder($"{_configuration["Applications:Tiktok:api"]}/{_configuration["Applications:Tiktok:version"]}/share/video/upload?access_token={socialProfile.access_token}");

                                    var _httpClient = new HttpClient();
                                    var response = await _httpClient.PostAsync(_uriBuilder.Uri, content);
                                    if (response.IsSuccessStatusCode)
                                    {
                                        return new { error = false, message = "Successful" };
                                    }
                                    else
                                    {
                                        return new { error = false, message = response.Content.ReadAsStringAsync() };
                                    }
                                }
                            }
                        }
                    }

                    return new { error = false, message = "Successful" };
                }
                else
                {
                    return new { error = true, message = "Failed" };
                }
            }
            else
            {
                return new { error = true, message = "Failed" };
            }
        }
        catch (Exception ex)
        {
            return new { error = true, message = ex.Message };
        }
    }

    public async Task<object> handlerGetProfiles()
    {
        try
        {
            var response = await _context.social_profiles.ToListAsync();
            string reponseString = JsonConvert.SerializeObject(response);
            return new { error = false, result = reponseString };
        }
        catch (Exception ex)
        {
            return new { error = true, message = ex.Message };
        }
    }
}
