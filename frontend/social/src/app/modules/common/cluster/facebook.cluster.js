import facebook from "fb";
import requestPromise from "request-promise";

const fbversion = "v18.0";

function Facebook(facebook_api) {
  this.facebook_api = facebook_api;
}

Facebook.prototype.addFacebookProfile = function (
  network,
  teamId,
  code,
  redirectUrl
) {
  return new Promise((resolve, reject) => {
    if (!code) {
      reject("Facebook code is invalid!");
    } else {
      this.getProfileAccessToken(code, redirectUrl)
        .then((accessToken) => {
          if (!accessToken) throw new Error("Cant able to fetch access token.");
          else {
            this.userProfileInfo(accessToken)
              .then((userDetails) => {
                if (!userDetails) {
                  throw new Error("Cant able to fetch user details");
                } else {
                  const user = {
                    UserName: userDetails.user_id,
                    FirstName: userDetails.first_name,
                    LastName: userDetails.last_name
                      ? userDetails.last_name
                      : "",
                    Email: userDetails.email,
                    SocialId: userDetails.user_id,
                    ProfilePicture: `https://graph.facebook.com/${userDetails.user_id}/picture?type=large`,
                    ProfileUrl: `https://facebook.com/${userDetails.user_id}`,
                    AccessToken: userDetails.access_token,
                    RefreshToken: userDetails.access_token,
                    FriendCount: userDetails.friend_count,
                    Info: "",
                    TeamId: teamId,
                    Network: network,
                  };

                  resolve(user);
                }
              })
              .catch((error) => {
                throw new Error(error.message);
              });
          }
        })
        .catch((error) => {
          reject(error.message);
        });
    }
  });
};

Facebook.prototype.getProfileAccessToken = function (code, redirecturl) {
  return new Promise((resolve, reject) => {
    if (!code) {
      reject("Invalid code from facebook");
    } else {
      const postOptions = {
        method: "GET",
        uri: `https://graph.facebook.com/${fbversion}/oauth/access_token`,
        // Setting JSON inputs to query
        qs: {
          client_id: this.facebook_api.app_id,
          redirect_uri: redirecturl,
          client_secret: this.facebook_api.secret_key,
          code,
        },
      };

      return requestPromise(postOptions)
        .then((response) => {
          const parsedResponse = JSON.parse(response);

          resolve(parsedResponse.access_token);
        })
        .catch((error) => {
          reject(error);
        });
    }
  });
};

Facebook.prototype.userProfileInfo = function (accessToken) {
  const url = `https://graph.facebook.com/${fbversion}/me?fields=id,ids_for_apps,name,email,birthday,first_name,last_name,friends&access_token=${accessToken}`;

  return new Promise((resolve, reject) =>
    request.get(url, (error, response, body) => {
      if (error) {
        reject(error);
      } else {
        const parsedBody = JSON.parse(body);
        const profileInfo = {
          user_id: parsedBody.id,
          name: parsedBody.name,
          email: parsedBody.email,
          birthday: parsedBody.birthday,
          first_name: parsedBody.first_name,
          last_name: parsedBody.last_name,
          friend_count: parsedBody.friends
            ? parsedBody.friends.summary
              ? parsedBody.friends.summary.total_count
              : "0"
            : "0",
          access_token: accessToken,
        };

        resolve(profileInfo);
      }
    })
  );
};
