export default function FacebookController(
  $scope,
  $http,
  $location,
  $window,
  $state,
  $sce,
  moment,
  API_CONFIG
) {
  // config
  const config = {
    headers: { "Content-Type": "application/json" },
  };
  const configFormData = {
    headers: {
      "Content-Type": undefined,
    },
    transformRequest: angular.identity,
  };

  // declare
  $scope.socialForm = {};
  $scope.profiles = [];
  $scope.files = [];
  $scope.fileCaches = [];
  $scope.videos = [];
  $scope.videoCaches = [];
  $scope.clickChange = false;
  let sendding = false;

  // function profile
  $scope.handlerLogin = (network) => {
    let redirect_url = "";
    switch (network) {
      case "facebook":
        // const facebook_api_key = "893904665525179";
        // const facebook_app_id = "893904665525179";
        // const facebook_client_id = "893904665525179";
        // const facebook_login_id = "a039803a-18df-4f8e-a1a5-e12946b026fd"; // genkey

        // const facebook_login_url = `https://www.facebook.com/login.php`;
        // const facebook_auth_url = `https://www.facebook.com/v18.0/dialog/oauth`;
        // const facebook_callback_url = "https://localhost:7118/api/facebook/callback";
        // const facebook_scopes = "read_insights,pages_manage_ads,pages_manage_metadata,pages_read_engagement,pages_read_user_content,ads_read,pages_manage_posts,pages_manage_engagement,pages_show_list,ads_management,pages_messaging,business_management,catalog_management,publish_to_groups,groups_access_member_info";

        // const facebook_login_uri = `${facebook_login_url}?skip_api_login=1&api_key=${facebook_api_key}&kid_directed_site=0&app_id=${facebook_app_id}&signed_next=1`;
        // const facebook_redirect_uri = `${facebook_callback_url}&scope=${facebook_scopes}&ret=login&fbapp_pres=0&logger_id=${facebook_login_id}&tp=unspecified`;
        // const facebook_next_url = `${facebook_auth_url}?state=2566861|2206045|fan&client_id=${facebook_client_id}&redirect_uri=${facebook_redirect_uri}`;
        // const facebook_cancel_url = `${facebook_callback_url}?error=access_denied&error_code=200&error_description=Permissions+error&error_reason=user_denied&state=2566861|2206045|fan`;

        // redirect_url = `${facebook_login_uri}&next=${facebook_next_url}&cancel_url=${facebook_cancel_url}&display=page&locale=en_GB&pl_dbl=0`;

        const facebook_client_id = "893904665525179";

        const facebook_auth_url = "https://www.facebook.com/v18.0/dialog/oauth";
        const facebook_callback_url = `https://localhost:7118/api/facebook/callback`;
        const facebook_scopes =
          "read_insights,pages_manage_ads,pages_manage_metadata,pages_read_engagement,pages_read_user_content,ads_read,pages_manage_posts,pages_manage_engagement,pages_show_list,ads_management,pages_messaging,business_management,catalog_management,publish_to_groups,groups_access_member_info";

        const facebook_login_uri = `${facebook_auth_url}?state=2566861|2206045|fan&client_id=${facebook_client_id}`;
        const facebook_redirect_uri = `${facebook_callback_url}&scope=${facebook_scopes}&ret=login&fbapp_pres=0&logger_id=${generateKey(
          16
        )}&tp=unspecified`;

        redirect_url = `${facebook_login_uri}&redirect_uri=${facebook_redirect_uri}`;
        break;
      case "youtube":
        const youtube_client_id =
          "23219178779-r88htl4bkh6mj43v4nk9ul890dp9qro4.apps.googleusercontent.com";

        const youtube_login_url =
          "https://accounts.google.com/o/oauth2/auth/oauthchooseaccount";
        const youtube_callback_url = `https://localhost:7118/api/youtube/callback`;
        const youtube_scopes =
          "https://www.googleapis.com/auth/youtube.upload https://www.googleapis.com/auth/youtube https://www.googleapis.com/auth/youtube.force-ssl https://www.googleapis.com/auth/userinfo.profile https://www.googleapis.com/auth/yt-analytics.readonly";

        const youtube_login_uri = `${youtube_login_url}?response_type=code&client_id=${youtube_client_id}`;
        const youtube_redirect_uri = `${youtube_callback_url}&scope=${youtube_scopes}&state=cfd9bccce49013bdd2ade90d78d45fe9&access_type=offline&include_granted_scopes=true&prompt=consent&service=lso&o2v=1&theme=glif&flowName=GeneralOAuthFlow`;

        redirect_url = `${youtube_login_uri}&redirect_uri=${youtube_redirect_uri}`;
        break;
      case "instagram":
        const instagram_client_id = "893904665525179";
        const instagram_login_url = "https://www.instagram.com/accounts/login";
        const instagram_callback_url =
          "https://localhost:7118/api/instagram/callback";
        const instagram_scopes = "user_profile,user_media";

        const instagram_login_uri = `${instagram_login_url}?force_authentication=1&enable_fb_login=1&next=/oauth/authorize?client_id=${instagram_client_id}&response_type=code`;
        const instagram_redirect_uri = `${instagram_callback_url}&scope=${instagram_scopes}&logger_id=03049d8a-c6f0-4152-b668-e79bbc618450`;

        redirect_url = `${instagram_login_uri}&redirect_uri=${instagram_redirect_uri}`;
        break;
      case "zalo":
        break;
      case "tiktok":
        const tiktok_client_key = "7312953301359216646";

        const tiktok_login_url = `https://www.tiktok.com/login`;
        const tiktok_auth_url = `https://www.tiktok.com/v2/auth/authorize`;
        const tiktok_callback_url = `https://localhost:7118/api/tiktok/callback`;
        const tiktok_scopes = "user.info.basic, user.insights, video.list, video.insights, video.publish, comment.list, comment.list.manage, user.info.username, user.info.stats";

        const tiktok_auth_uri = `${tiktok_login_url}?lang=vi&enter_method=web&enter_from=dev_${tiktok_client_key}`;
        const tiktok_redirect_uri = `${tiktok_callback_url}&state={"group_id": 2206045, "auth_version": "2.0"}`;
        const tiktok_redirect_url = `${tiktok_auth_url}?client_key=${tiktok_client_key}&scope=${tiktok_scopes}&response_type=code&redirect_uri=${tiktok_redirect_uri}`;

        redirect_url = `${tiktok_auth_uri}&redirect_url=${tiktok_redirect_url}&hide_left_icon=0&type=`;
        break;
      case "googleanalytics":
        break;
      default:
        break;
    }
    window.open(redirect_url, "_self");
  };
  const generateKey = (length) => {
    const characters =
      "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    let result = "";
    for (let i = 0; i < length; i++) {
      const randomIndex = Math.floor(Math.random() * characters.length);
      result += characters.charAt(randomIndex);
    }
    return result;
  };
  const addProfile = (postData) => {
    if (
      [
        "facebook",
        "youtube",
        "instagram",
        "tiktok",
        "twitter",
        "googleanalytics",
      ].includes(postData.profile_type) !== true
    ) {
      $window.history.back();
      return;
    }
    let redirect_url = "";
    $http
      .post(
        `${API_CONFIG}/api/${postData.profile_type}/addProfile`,
        postData,
        config
      )
      .then(function (response) {
        if (response.data.error) {
          alert(`Error post: ${response.data.message}`);
        }
        redirect_url = response.data.redirect_url;
      })
      .then(function () {
        // $state.go("social");
        window.open(redirect_url, "_self");
      })
      .catch(function (error) {
        alert(`Error post data: ${error}`);
      });
  };
  $scope.deleteProfile = (profile) => {
    if (confirm("Bạn có chắc muốn xoá hồ sơ này?")) {
      $http
        .delete(`${API_CONFIG}/api/${profile.profile_type}/deleteProfile`, {
          params: { slug: profile.key_id },
          headers: config.headers,
        })
        .then(function () {
          loadProfiles();
        })
        .catch(function (error) {
          alert(`Error delete data: ${error}`);
        });
    }
  };

  // function choose file
  $scope.chooseFile = (id) => {
    $scope.clickChange = true;
    document.getElementById(id).click();
  };
  $scope.uploadFile = () => {
    if ($scope.files != null && $scope.files.length > 0) {
      for (var i = 0; i < $scope.files.length; i++) {
        let file = $scope.files[i];
        file.url = URL.createObjectURL(file);
        file.key = "image";
        $scope.fileCaches.push(file);
      }
    }
    $scope.files = [];
    $scope.clickChange = false;
  };
  $scope.uploadVideo = () => {
    if ($scope.videos != null) {
      let video = $scope.videos;
      video.url = URL.createObjectURL(video);
      video.key = "video";
      $scope.videoCaches = [video];
    }
    $scope.videos = [];
    $scope.clickChange = false;
  };
  $scope.deleteFile = (idx) => {
    if ($scope.fileCaches != null && $scope.fileCaches.length > 0) {
      $scope.fileCaches.splice(idx, 1);
    }
  };
  $scope.deleteVideo = (idx) => {
    if ($scope.videoCaches != null && $scope.videoCaches.length > 0) {
      $scope.videoCaches.splice(idx, 1);
    }
  };
  $scope.toogleChooseFile = () => {
    document.getElementById("myDropdown").classList.toggle("hidden");
  };
  window.onclick = (event) => {
    if (!event.target.matches(".btnDrop")) {
      let dropdowns = document.getElementsByClassName("dropdown-content");
      for (var i = 0; i < dropdowns.length; i++) {
        let openDropdown = dropdowns[i];
        if (!openDropdown.classList.contains("hidden")) {
          openDropdown.classList.add("hidden");
        }
      }
    }
  };

  // function html
  $scope.trustAsHtml = (html) => {
    if (!html) {
      return "";
    }
    return urlify(html).replace(/\n/g, "<br/>");
  };
  const urlify = (string) => {
    var urlRegex = string.match(
      /((((ftp|https?):\/\/)|(w{3}\.))[\-\w@:%_\+.~#?,&\/\/=]+)/g
    );
    if (urlRegex) {
      urlRegex.forEach(function (url) {
        string = string.replace(
          url,
          '<a target="_blank" href="' + url + '">' + url + "</a>"
        );
      });
    }
    return string.replace("(", "<br/>(");
  };

  // function public post
  $scope.publicPost = () => {
    if (sendding) {
      return;
    }
    sendding = true;
    let socialForm = Object.assign($scope.socialForm);
    let socialProfiles = [...$scope.profiles.filter((x) => x.checked === true)];

    let formData = new FormData();
    if ($scope.fileCaches != null && $scope.fileCaches.length > 0) {
      $scope.fileCaches.forEach((file) => {
        formData.append(file.key, file);
      });
      socialForm.post_type = "Image";
    }
    if ($scope.videoCaches != null && $scope.videoCaches.length > 0) {
      $scope.videoCaches.forEach((video) => {
        formData.append(video.key, video);
      });
      socialForm.post_type = "Video";
    }
    formData.append("socialForm", JSON.stringify(socialForm));
    formData.append("socialProfiles", JSON.stringify(socialProfiles));
    $http
      .post(`${API_CONFIG}/api/social/publicPost`, formData, configFormData)
      .then(function (response) {
        sendding = false;
        resetForm();
        if (response.data.error) {
          alert(`Error public post: ${response.data.error}`);
        }
        alert(`Public post successful`);
      })
      .catch(function (error) {
        sendding = false;
        alert(`Error public post: ${error}`);
      });
  };
  const resetForm = () => {
    if ($scope.profiles != null && $scope.profiles.length > 0) {
      $scope.profiles
        .filter((x) => x.checked === true)
        .forEach(function (profile) {
          profile.checked = false;
        });
    }
    $scope.socialForm = {
      post_type: "Text",
      published: "true",
      create_date: moment(new Date()).format("DD/MM"),
    };
    $scope.fileCaches = [];
    $scope.videoCaches = [];
  };

  // initData
  const loadProfiles = () => {
    $scope.profiles = [];
    $http
      .get(`${API_CONFIG}/api/social/getProfiles`)
      .then(function (response) {
        if (response.data.error) {
          alert(`Error data: ${response.data.message}`);
        } else {
          const data = response.data;
          if (data && data.result) {
            let result = JSON.parse(data.result);
            $scope.profiles = result;
          }
        }
      })
      .catch(function (error) {
        alert(`Error loading data: ${error}`);
      });
  };

  // initOne
  this.initOne = () => {
    const params = $location.search();
    if (params.profile_type != null && params.access_token != null) {
      addProfile(params);
    } else {
      loadProfiles();
    }
    resetForm();
  };
  this.initOne();
}

// Đăng ký các phụ thuộc (nếu cần thiết)
FacebookController.$inject = [
  "$scope",
  "$http",
  "$location",
  "$window",
  "$state",
  "$sce",
  "moment",
  "API_CONFIG",
];
