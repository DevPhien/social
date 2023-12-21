using Microsoft.Extensions.Configuration;
using System.Net;
using Tweetinvi;
using Tweetinvi.Auth;
using Tweetinvi.Credentials.Models;
using Tweetinvi.Models;
using Tweetinvi.Parameters;

namespace social.Services.Twitter;
public class TwitterHandler : ITwitterHandler
{
    private readonly IConfiguration _configuration;
    public TwitterHandler(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    //public async Task<object> handlerGetFollowsOfUserIDAsync()
    //{
    //    var client = new HttpClient();
    //    var request = new HttpRequestMessage(HttpMethod.Get, "https://api.twitter.com/2/users/1732591760776261958/followers");
    //    request.Headers.Add("id", "1732591760776261958");
    //    request.Headers.Add("Authorization", "OAuth oauth_consumer_key=\"" + _configuration.GetSection("Authentication:Twitter:ConsumerAPIKey") + "\",oauth_token=\"" + _configuration.GetSection("Authentication:Twitter:ConsumerSecret") + "\",oauth_signature_method=\"HMAC-SHA1\",oauth_timestamp=\"1701935415\",oauth_nonce=\"ipl6NzMsWuC\",oauth_version=\"1.0\",oauth_signature=\"valMG2eLtljR0g5%2F5BZckTIz4uU%3D\"");
    //    request.Headers.Add("Cookie", "guest_id=v1%3A170192511769979205; guest_id_ads=v1%3A170192511769979205; guest_id_marketing=v1%3A170192511769979205; personalization_id=\"v1_jVgvew5380BRBPCZVbAKvw==\"");
    //    var response = await client.SendAsync(request);
    //    response.EnsureSuccessStatusCode();
    //    var httpContent = await response.Content.ReadAsStringAsync();

    //    return new { httpContent };
    //}
    //public async Task<object> handelrPostTweestAsync()
    //{
    //    var client = new HttpClient();
    //    var request = new HttpRequestMessage(HttpMethod.Post, "https://api.twitter.com/2/users/1732591760776261958/retweets?text=test");
    //    request.Headers.Add("Authorization", "OAuth oauth_consumer_key=\"" + _configuration.GetSection("Authentication:Twitter:ConsumerAPIKey") + "\",oauth_token=\"" + _configuration.GetSection("Authentication:Twitter:ConsumerSecret") + "\",oauth_signature_method=\"HMAC-SHA1\",oauth_timestamp=\"1701935415\",oauth_nonce=\"ipl6NzMsWuC\",oauth_version=\"1.0\",oauth_signature=\"valMG2eLtljR0g5%2F5BZckTIz4uU%3D\"");
    //    request.Headers.Add("Cookie", "guest_id=v1%3A170192511769979205; guest_id_ads=v1%3A170192511769979205; guest_id_marketing=v1%3A170192511769979205; personalization_id=\"v1_jVgvew5380BRBPCZVbAKvw==\"");
    //    var content = new StringContent("{\n\"tweet_id\": \"1415348607813832708\"\n}", null, "application/json");
    //    request.Content = content;
    //    var response = await client.SendAsync(request);
    //    response.EnsureSuccessStatusCode();
    //    var httpContent = await response.Content.ReadAsStringAsync();

    //    return new { httpContent };
    //}
}
