namespace social.Entites
{
    public class YoutubeProfile
    {
        // Mã tài khoản
        public string client_id { get; set; } = null!;
        // Mã bí mật
        public string client_secret { get; set; } = null!;
        // Đường dãn trả về mã code
        public string redirect_url { get; set; } = "https://localhost:7118/api/youtube/youtube-callback";
        // Đường dãn trả về mã code hồ sơ
        public string profile_add_redirect_url { get; set; } = "https://localhost:7118/api/youtube/callback";
        // Khoá api đăng ký
        public string api_key { get; set; } = "";
        public string invite_redirect_url { get; set; } = "";
        // Quyền nhóm
        public string group_scopes { get; set; } = "";
        // Quyền đăng nhập
        public string login_scopes { get; set; } = "https%3A%2F%2Fwww.googleapis.com%2Fauth%2Fuserinfo.email+https%3A%2F%2Fwww.googleapis.com%2Fauth%2Fuserinfo.profile";


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

    public class YoutubeVideo
    {
        // Mã video
        public string video_id { get; set; } = null!;
        // Mã kênh
        public string channel_ìd { get; set; } = null!;
        // Tiêu đề kênh
        public string? channel_title { get; set; }
        // Tiêu đề video
        public string? title { get; set; }
        // Mô tả
        public string? description { get; set; }
        // Ngày công khai video
        public DateTime? published_at { get; set; }
        // Đường dẫn ảnh thu nhỏ
        public string? thumbnail_url { get; set; }
        // Đường dãn video nhúng
        public string embed_url { get; set; } = null!;
        // Tag
        public string? etag { get; set; }
        // Đã thích: "like, dislike, none"
        public string is_liked { get; set; } = "none";
        // Đường dẫn ảnh
        public string media_url { get; set; } = null!;
        // Phiên bản
        public string? version { get; set; }
        // Ngày tạo
        public DateTime? create_date { get; set; }
        // Ngày cập nhật
        public DateTime? update_date { get; set; }
    }
}