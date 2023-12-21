namespace social.Entites;

public class InstagramAPI
{
    public string client_id { get; set; } = null!;

    public string client_secret { get; set; } = null!;
}

public class InstagramForm
{
    public string client_id { get; set; } = null!;

    public string client_secret { get; set; } = null!;

    public string? redirect_uri { get; set; }

    public string? code { get; set; }
}

public class InstagramProfile
{
    public string? username { get; set; }
    public string? email { get; set; }
    public string? birthday { get; set; }
    public string? first_name { get; set; }
    public string? last_name { get; set; }
    public string? profile_url { get; set; }
    public int friend_count { get; set; } = 0;
    public int access_token { get; set; }
    
}

public class InstagramPost
{
    public string feedId { get; set; } = null!;
    public string? privacy { get; set; }
    public DateTime? created_time { get; set; }
    public int type { get; set; }
    public string? feed_message { get; set; }
    public string? feed_link { get; set; }
    public bool is_bool { get; set; }
    public bool is_string { get; set;}
    public List<InstagramMedia> media_urls { get; set; } = new List<InstagramMedia>();
    public int likeCount { get; set; } = 0;
    public int commentCount { get; set; } = 0;
    public string? social_id { get; set; } = null!;
    public string? shared_url { get; set; }
}

public class InstagramMedia
{
    public string url { get; set; } = null!;
}