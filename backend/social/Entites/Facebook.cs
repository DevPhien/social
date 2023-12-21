namespace social.Entites;

public class FacebookProfile
{
    // Mã ứng dụng
    public string client_id { get; set; } = null!;
    // Mã bí mật
    public string? client_secret { get; set; }
    // Đường dãn trả về mã code
    public string redirect_url { get; set; } = "https://localhost:7118/api/facebook/facebook-callback";
    // Đường dãn trả về mã code hồ sơ
    public string profile_add_redirect_url { get; set; } = "https://localhost:7118/api/facebook/callback";
    public string invite_redirect_url { get; set; } = "";
    // Quyền page
    public string page_scopes { get; set; } = "user_posts,publish_pages,manage_pages,read_insights";
    // Quyền group
    public string group_scopes { get; set; } = "";
    // Quyền login
    public string login_scopes { get; set; } = "email,user_posts,user_birthday,publish_pages,manage_pages,publish_to_groups,user_friends";
    // Quyền profile
    public string profile_scopes { get; set; } = "email";

    // Mã tài khoản
    public string? id { get; set; }
    // Tên tài khoản
    public string? name { get; set; }
    // Email tài khoản
    public string? email { get; set; }
    // Ngày sinh
    public string? birthday { get; set; }
    // Họ
    public string? first_name { get; set; }
    // Tên
    public string? last_name { get; set; }
    // Số lượng bàn bè
    public int friend_count { get; set; } = 0;
    // Token truy cập
    public string? access_token { get; set; }
    // Token tải lại
    public string? refresh_token { get; set; }
}

public class FacebookForm
{
    public string client_id { get; set; } = null!;
    // Mã bí mật
    public string client_secret { get; set; } = null!;

}

public class FacebookFeed
{
    public string feed_id { get; set; } = null!;
    // Riêng tư
    public bool privacy { get; set; } = false;
    // Ngày tạo
    public DateTime? created_time { get; set; }
    // Loại bài viết: "photo,album, video, video_inline, profile_media, cover_photo, share"
    public string? feed_type { get; set; }
    // Nội dung bài viết
    public string? feed_message { get; set; }
    // Đường dẫn bài viết
    public string? feed_link { get; set; }
    // Đăng từ trên điện thoại
    public bool is_application { get; set; }
    // Danh sách đường dẫn file đính kèm
    public List<FacebookFeedMedia>? media_urls { get; set; }
    // Tổng số like
    public int like_count { get; set; } = 0;
    // Tổng số bình luận
    public int comment_count { get; set; } = 0;
    // Đường dẫn chia sẻ bài viết
    public string? shared_url { get; set; }
} 

public class FacebookFeedMedia
{
    // Đường dẫn file đính kèm
    public string url { get; set; } = null!;
}