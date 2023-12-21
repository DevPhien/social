namespace social.Entites;

public partial class social_profile
{
    // GenKey
    public string key_id { get; set; } = null!;
    // Loại hồ sơ: "facebook, youtube, instagram"
    public string profile_type { get; set; } = null!;
    // Khoá api (nếu có)
    public string? api_key { get; set; }
    // Mã page (nếu có)
    public string? page_id { get; set; }
    // Mã tài khoản
    public string? id { get; set; }
    // Tên tài khoản
    public string? name { get; set; }
    // Email tài khoản
    public string? email { get; set; }
    // Ngày sinh
    public DateTime? birthday { get; set; }
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

public class SocialForm
{
    // Loại bài viết
    public string? post_type { get; set; } // ["Text", "OldImage", "Image", "Link", "Video"];
    // Nội dung bài viết
    public string? message { get; set; }
    // Mô tả bài viết
    public string? description { get; set; }
    // Nhãn
    public string? tags { get; set; }
    // Đường dẫn
    public string? link { get; set; }
    // Công khai
    public string? published { get; set; }
}

public class SocialResult
{
    public string? profile_type { get; set; }
    public string? access_token { get; set; }
    public string? refresh_token { get; set; }
    public string? expires_in { get; set; }
    public string? refresh_expires_in { get; set; }
    public string? scope { get; set; }
}