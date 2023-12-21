//using Google.Apis.AnalyticsReporting.v4;
//using Google.Apis.AnalyticsReporting.v4.Data;
//using Google.Apis.Auth.OAuth2;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using Tweetinvi.Models;

//class Program
//{
//    static void Main(string[] args)
//    {
//        // Đường dẫn đến tệp tin JSON chứa thông tin xác thực
//        string keyFilePath = "path/to/your/service-account-key.json";

//        // ID của tài khoản Google Analytics
//        string viewId = "your-analytics-view-id";

//        // Xác thực
//        GoogleCredential credential;
//        using (var stream = new FileStream(keyFilePath, FileMode.Open, FileAccess.Read))
//        {
//            credential = GoogleCredential.FromStream(stream)
//                .CreateScoped(AnalyticsReportingService.Scope.AnalyticsReadonly);
//        }

//        // Tạo dịch vụ Analytics
//        using (var service = new AnalyticsReportingService(new Google.Apis.Services.BaseClientService.Initializer
//        {
//            HttpClientInitializer = credential
//        }))
//        {
//            // Truy vấn dữ liệu từ API
//            var reportRequest = new ReportRequest
//            {
//                ViewId = viewId,
//                DateRanges = new List<DateRange> { new DateRange { StartDate = "7daysAgo", EndDate = "today" } },
//                Metrics = new List<Metric> { new Metric { Expression = "ga:sessions" } },
//                Dimensions = new List<Dimension> { new Dimension { Name = "ga:source" } }
//            };

//            var getReportsRequest = new GetReportsRequest
//            {
//                ReportRequests = new List<ReportRequest> { reportRequest }
//            };

//            var batchRequest = service.Reports.BatchGet(getReportsRequest);
//            var response = batchRequest.Execute();

//            // In kết quả
//            foreach (var report in response.Reports)
//            {
//                Console.WriteLine("Report:");
//                foreach (var columnHeader in report.ColumnHeader.Dimensions)
//                {
//                    Console.Write(columnHeader + "\t");
//                }
//                foreach (var metricHeader in report.ColumnHeader.MetricHeader.MetricHeaderEntries)
//                {
//                    Console.Write(metricHeader.Name + "\t");
//                }
//                Console.WriteLine();

//                foreach (var row in report.Data.Rows)
//                {
//                    foreach (var dimension in row.Dimensions)
//                    {
//                        Console.Write(dimension + "\t");
//                    }
//                    foreach (var metric in row.Metrics)
//                    {
//                        Console.Write(metric.Values[0] + "\t");
//                    }
//                    Console.WriteLine();
//                }
//            }
//        }
//    }
//}


using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using IdentityModel.Client;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Tweetinvi.Core.Models;
using Tweetinvi.Models;

namespace social.Services.Youtube;

public class YoutubeHandler : IYoutubeHandler
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;
    private readonly SocialContext _context;

    public YoutubeHandler(IConfiguration configuration, IWebHostEnvironment environment, SocialContext context)
    {
        _configuration = configuration;
        _environment = environment;
        _context = context;
    }

    public async Task<object> handlerAddYoutubeProfileAsync(string? access_token, string? refresh_token)
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
                    profile.profile_type = "youtube";
                    profile.access_token = access_token;
                    profile.refresh_token = refresh_token;
                    _context.social_profiles.Add(profile);
                    await _context.SaveChangesAsync();
                }
                return new { error = false, message = "Successful", redirect_url = _configuration["Applications:Youtube:redirect_url"] };
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

    public async Task<object> handlerDeleteYoutubeProfileAsync(string slug)
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

    //public async Task<SocialProfile?> handlerGetYoutubeProfileAsync(string slug)
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

    //public async Task<List<SocialProfile>?> handlerGetYoutubeProfilesAsync(string search, int pageNo, int pageSize)
    //{
    //    var profiles = await _context.SocialProfiles.Where(x => x.profile_type == "youtube" && (x.name ?? "").Contains(search)).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();
    //    if (profiles is not null && profiles.Count > 0)
    //    {
    //        return profiles;
    //    }
    //    else
    //    {
    //        return null;
    //    }
    //}

    public string handlerGetUrlAccessTokenTokenAsync()
    {
        var authUri = "https://accounts.google.com/o/oauth2/v2/auth";
        var clientId = _configuration["Applications:Youtube:client_id"];
        var scope = "https://www.googleapis.com/auth/youtube https://www.googleapis.com/auth/youtube.channel-memberships.creator https://www.googleapis.com/auth/youtube.force-ssl https://www.googleapis.com/auth/youtube.readonly https://www.googleapis.com/auth/youtube.upload https://www.googleapis.com/auth/youtubepartner https://www.googleapis.com/auth/youtubepartner-channel-audit";
        return $"{authUri}?client_id={clientId}&redirect_uri={_configuration["Applications:Youtube:redirect_add_profile_url"]}&prompt=consent&response_type=code&scope={scope}";
    }

    public async Task<string> handlerGetProfileAccessTokenAsync(string code)
    {
        try
        {
            var client = new HttpClient();
            var tokenResponse = await client.RequestAuthorizationCodeTokenAsync(new AuthorizationCodeTokenRequest
            {
                Address = "https://oauth2.googleapis.com/token",
                ClientId = _configuration["Applications:Youtube:client_id"],
                ClientSecret = _configuration["Applications:Youtube:client_secret"],
                Code = code,
                RedirectUri = _configuration["Applications:Youtube:redirect_add_profile_url"],
                GrantType = "authorization_code",
                Parameters =
                {
                    { "access_type", "offline" },
                    { "prompt", "consent" },
                    { "approval_prompt", "force" }
                }
            });

            return $"{_configuration["Applications:Youtube:redirect_url"]}?profile_type=youtube&access_token={tokenResponse.AccessToken}&refresh_token={tokenResponse.RefreshToken}";
        }
        catch {
            return $"{_configuration["Applications:Youtube:redirect_url"]}";
        }
    }

    public async Task<social_profile?> handlerGetProfileInfo(string? accessToken, string? refresh_token)
    {
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

            var response = await client.GetAsync("https://www.googleapis.com/youtube/v3/channels?part=snippet&mine=true");
            if (response.IsSuccessStatusCode)
            {
                var profile = new social_profile();
                var result = JsonConvert.DeserializeObject<JObject>(await response.Content.ReadAsStringAsync());
                if (result is not null)
                {
                    var items = JsonConvert.DeserializeObject<List<JObject>>(result?["items"]?.ToString() ?? "[]");
                    if (items is not null && items.Count > 0)
                    {
                        var item = items[0];
                        if (item is not null)
                        {
                            
                            profile.id = item?["id"]?.ToString() ?? "";
                            var snippet = JsonConvert.DeserializeObject<JObject>(item?["snippet"]?.ToString() ?? "{}");
                            profile.name = snippet?["title"]?.ToString() ?? "";
                        }
                    } 
                }

                return profile;
            }
            else
            {
                return null;
            }
        }
    }

    void videosInsertRequest_ProgressChanged(Google.Apis.Upload.IUploadProgress progress)
    {
        switch (progress.Status)
        {
            case UploadStatus.Uploading:
                Console.WriteLine("{0} bytes sent.", progress.BytesSent);
                break;

            case UploadStatus.Failed:
                Console.WriteLine("An error prevented the upload from completing.\n{0}", progress.Exception);
                break;
        }
    }

    void videosInsertRequest_ResponseReceived(Video video)
    {
        Console.WriteLine("Video id '{0}' was successfully uploaded.", video.Id);
    }

    public async Task<object> handlerUploadVideo(string accessToken, string? refreshToken, string videoPath)
    {
        try
        {
            string root = _environment.WebRootPath;

            var token = new Google.Apis.Auth.OAuth2.Responses.TokenResponse { AccessToken = accessToken, RefreshToken = refreshToken };
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

            //UserCredential credential;
            //using (var stream = new FileStream(root + "/Portals/Youtube/client_secret_23219178779-r88htl4bkh6mj43v4nk9ul890dp9qro4.apps.googleusercontent.com.json", FileMode.Open, FileAccess.Read))
            //{
            //    credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
            //        GoogleClientSecrets.Load(stream).Secrets,
            //        // This OAuth 2.0 access scope allows an application to upload files to the
            //        // authenticated user's YouTube channel, but doesn't allow other types of access.
            //        new[] { YouTubeService.Scope.YoutubeUpload },
            //        "user",
            //        CancellationToken.None
            //    );
            //}

            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                // ApiKey = _configuration["Applications:Youtube:api_key"],
                HttpClientInitializer = credential,
                ApplicationName = Assembly.GetExecutingAssembly().GetName().Name
            });

            var video = new Video();
            video.Snippet = new VideoSnippet();
            video.Snippet.Title = "Default Video Title";
            video.Snippet.Description = "Default Video Description";
            video.Snippet.Tags = new string[] { "tag1", "tag2" };
            //video.Snippet.CategoryId = "22"; // See https://developers.google.com/youtube/v3/docs/videoCategories/list
            video.Status = new VideoStatus();
            video.Status.PrivacyStatus = "public"; // or "private" or "public"
            var filePath = root + "/Portals/Youtube/video4k.mp4";

            using (var fileStream = new FileStream(filePath, FileMode.Open))
            {
                var videosInsertRequest = youtubeService.Videos.Insert(video, "snippet,status", fileStream, "video/*");
                videosInsertRequest.ProgressChanged += videosInsertRequest_ProgressChanged;
                videosInsertRequest.ResponseReceived += videosInsertRequest_ResponseReceived;

                var responseBody = await videosInsertRequest.UploadAsync();

                return new { error = false, result = responseBody };
            }

            //// Add a video to the newly created playlist.
            //var newPlaylistItem = new PlaylistItem();
            //newPlaylistItem.Snippet = new PlaylistItemSnippet();
            //newPlaylistItem.Snippet.PlaylistId = newPlaylist.Id;
            //newPlaylistItem.Snippet.ResourceId = new ResourceId();
            //newPlaylistItem.Snippet.ResourceId.Kind = "youtube#video";
            //newPlaylistItem.Snippet.ResourceId.VideoId = "GNRMeaz6QRI";
            //newPlaylistItem = await youtubeService.PlaylistItems.Insert(newPlaylistItem, "snippet").ExecuteAsync();


            //string[] scopes = new string[] {
            //    YouTubeService.Scope.Youtube,
            //    YouTubeService.Scope.YoutubeUpload
            //};

            //var token = new Google.Apis.Auth.OAuth2.Responses.TokenResponse
            //{
            //    AccessToken = accessToken,
            //    RefreshToken = refreshToken,
            //};

            //var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            //{
            //    ClientSecrets = new ClientSecrets
            //    {
            //        ClientId = _configuration["Applications:Youtube:client_id"],
            //        ClientSecret = _configuration["Applications:Youtube:client_secret"],
            //    },
            //    Scopes = scopes,
            //    DataStore = new FileDataStore("Store"),
            //});

            //UserCredential credential = new UserCredential(flow, Environment.UserName, token);
            //if (!credential.RefreshTokenAsync(CancellationToken.None).Result)
            //{
            //    // Handle failed token refresh.
            //    return new { error = true, result = "Failed to refresh token." };
            //}

            //// Use the refreshed credential for API calls.
            //var youtubeService = new YouTubeService(new BaseClientService.Initializer
            //{
            //    ApiKey = _configuration["Applications:Youtube:api_key"],
            //    HttpClientInitializer = credential,
            //    ApplicationName = Assembly.GetExecutingAssembly().GetName().Name,
            //});


            //var video = new Video();
            //video.Snippet = new VideoSnippet();
            //video.Snippet.Title = "Default Video Title";
            //video.Snippet.Description = "Default Video Description";
            //video.Snippet.Tags = new string[] { "tag1", "tag2" };
            //// video.Snippet.CategoryId = "22"; // See https://developers.google.com/youtube/v3/docs/videoCategories/list
            //// video.Snippet.ChannelId = "UC1vW9MIBXC4UeKl2EcWjfxw";
            //video.Status = new VideoStatus();
            //video.Status.PrivacyStatus = "public"; // or "private" or "public"
            //string root = _environment.WebRootPath;
            //var filePath = root + "/Portals/Youtube/video4k.mp4";

            //using (var fileStream = new FileStream(filePath, FileMode.Open))
            //{
            //    var videosInsertRequest = youtubeService.Videos.Insert(video, "snippet,status", fileStream, "video/*");

            //    var responseBody = await videosInsertRequest.UploadAsync();

            //    return new { error = false, result = responseBody };
            //}


            //using (HttpClient client = new HttpClient())
            //{
            //    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

            //    videoPath = "/Portals/Youtube/video4k.mp4";

            //    using (var fileStream = new FileStream(root + videoPath, FileMode.Open))
            //    using (var content = new MultipartFormDataContent())
            //    {
            //        content.Add(new StreamContent(fileStream), "video", "video.mp4");

            //        var response = await client.PostAsync("https://www.googleapis.com/upload/youtube/v3/videos?part=snippet,status,contentDetails", content);
            //        var responseBody = await response.Content.ReadAsStringAsync();

            //        return new { error = false, result = responseBody };
            //    }
            //}


            //var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            //{
            //    ApiKey = _configuration["Applications:Youtube:api_key"],
            //    ApplicationName = Assembly.GetExecutingAssembly().GetName().Name
            //});

            //var video = new Video();
            //video.Snippet = new VideoSnippet();
            //video.Snippet.Title = "Default Video Title";
            //video.Snippet.Description = "Default Video Description";
            //video.Snippet.Tags = new string[] { "tag1", "tag2" };
            ////video.Snippet.CategoryId = categoryId;
            //video.Snippet.ChannelId = "UC1vW9MIBXC4UeKl2EcWjfxw";
            //videoPath = root + "/Portals/Youtube/video4k.mp4";
            //var videoInsertRequest = youtubeService.Videos.Insert(video, "snippet,status", File.OpenRead(videoPath), "video/*");
            //var uploadedVideo = await videoInsertRequest.UploadAsync();

            //return new { error = false, result = uploadedVideo };
        }
        catch (Exception ex)
        {
            return new { error = true, result = ex.Message };
        }
    }

    public async Task<object> handlerSetThumbnail(string access_token, string videoId, string thumbnailPath)
    {
        string root = _environment.WebRootPath;
        var queryParams = new Dictionary<string, string>
        {
            { "access_token", access_token },
            { "grant_type", "authorization_code" },
        };

        var _uriBuilder = new UriBuilder($"{_configuration["Applications:Youtube:api"]}/upload/youtube/v3/thumbnails/set?videoId={videoId}&uploadType=media");

        var _httpClient = new HttpClient();
        thumbnailPath = root + "/Partals/Youtube/download.jpg";
        using (var fileStream = new FileStream(thumbnailPath, FileMode.Open))
        using (var content = new MultipartFormDataContent())
        {
            content.Add(new StreamContent(fileStream), "file", "file.jpg");
        }
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

    public async Task<object> handlerGetInfo(string access_token)
    {
        try
        {
            var _uriBuilder = new UriBuilder($"{_configuration["Applications:Youtube:api"]}/youtube/v3/channels?part=snippet,contentDetails,statistics&id=UC1vW9MIBXC4UeKl2EcWjfxw&key={_configuration["Applications:Youtube:api_key"]}");

            var _client = new HttpClient();
            var response = await _client.GetAsync(_uriBuilder.Uri);

            string responseBody = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                return new { error = false, result = responseBody };
            }
            else
            {
                return new { error = true, result = responseBody };
            }
        }
        catch (Exception ex)
        {
            return new { error = true, result = ex.Message };
        }
    }

    public async Task<object> handlerGetVideos(string access_token)
    {
        try
        {
            var _uriBuilder = new UriBuilder($"{_configuration["Applications:Youtube:api"]}/youtube/v3/search?part=snippet&maxResults=10&order=date&type=video&key={_configuration["Applications:Youtube:api_key"]}");

            var _client = new HttpClient();
            var response = await _client.GetAsync(_uriBuilder.Uri);

            string responseBody = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                return new { error = false, result = responseBody };
            }
            else
            {
                return new { error = true, result = responseBody };
            }
        }
        catch (Exception ex)
        {
            return new { error = true, result = ex.Message };
        }
    }
    
    public async Task<object> handlerLikeVideo(string access_token, string videoId, string rating) // rating : 'like', 'dislike', 'none'
    {
        var _uriBuilder = new UriBuilder($"{_configuration["Applications:Youtube:api"]}/youtube/v3/videos/rate?id={videoId}&rating={rating}&access_token={access_token}");

        var _httpClient = new HttpClient();
        var response = await _httpClient.PostAsync(_uriBuilder.Uri, null);
        string responseBody = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
        {
            return new { error = false, message = "Successful" };
        }
        else
        {
            return new { error = true, message = responseBody };
        }
    }

    public async Task<object> handlerCommentVideo(string access_token, string videoId, string comment)
    {
        var postData = new
        {
            snippet = new
            {
                topLevelComment = new
                {
                    snippet = new
                    {
                        textOriginal = comment
                    }
                },
                videoId
            }
        };

        var _uriBuilder = new UriBuilder($"{_configuration["Applications:Youtube:api"]}/youtube/v3/videos/commentThreads?part=snippet&access_token={access_token}");
        var content = new StringContent(JsonConvert.SerializeObject(postData), Encoding.UTF8, "application/json");

        var _httpClient = new HttpClient();
        var response = await _httpClient.PostAsync(_uriBuilder.Uri, content);
        string responseBody = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
        {
            return new { error = false, message = "Successful" };
        }
        else
        {
            return new { error = true, message = responseBody };
        }
    }
}

