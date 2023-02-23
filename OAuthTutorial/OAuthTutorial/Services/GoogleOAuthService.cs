using Microsoft.AspNetCore.WebUtilities;
using OAuthTutorial.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OAuthTutorial.Services
{
    public class GoogleOAuthService
    {
        private const string ClientId = "197979692814-oio76eojdgt4v5rtmfapfvre19752cgf.apps.googleusercontent.com";
        private const string ClientSecret = "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";

        private const string ClientIdFacebook = "968685937912559";
        private const string ClientSecretFacebook = "76fb5d4c823e42b88132478de96fe02f";

        private const string OAuthServerEndpoint = "https://accounts.google.com/o/oauth2/v2/auth";
        private const string TokenServerEndpoint = "https://oauth2.googleapis.com/token";

        public static string GenerateOAuthRequestUrl(string scope, string redirectUrl, string codeChellange)
        {
            var queryParams = new Dictionary<string, string>
            {
                {"client_id", ClientId},
                { "redirect_uri", redirectUrl },
                { "response_type", "code" },
                { "scope",  scope },
                { "code_challenge", codeChellange },
                { "code_challenge_method", "S256" },
                { "access_type", "offline" }
            };

            var url = QueryHelpers.AddQueryString(OAuthServerEndpoint, queryParams);
            return url;
        }

        public static async Task<TokenResult> ExchangeCodeOnTokenAsync(string code, string codeVerifier, string redirectUrl)
        {
            var authParams = new Dictionary<string, string>
            {
                { "client_id", ClientId },
                { "client_secret", ClientSecret },
                { "code", code },
                { "code_verifier", codeVerifier },
                { "grant_type", "authorization_code" },
                { "redirect_uri", redirectUrl }
            };

            var tokenResult = await HttpClientHelper.SendPostRequest<TokenResult>(TokenServerEndpoint, authParams);
            return tokenResult;
        }

        public static string GenerateOAuthRequestUrlFacebook(string redirectUrl, string codeChellange)
        {
            var queryParams = new Dictionary<string, string>
            {
                { "client_id", ClientIdFacebook},
                { "redirect_uri", redirectUrl },
                { "scope",  "email" },
                { "code_challenge", codeChellange },
                { "code_challenge_method", "S256" }
            };

            var url = QueryHelpers.AddQueryString("https://www.facebook.com/dialog/oauth", queryParams);
            return url;
        }

        public static async Task<TokenResult> ExchangeCodeOnTokenFacebookAsync(string code, string codeVerifier, string redirectUrl)
        {
            var authParams = new Dictionary<string, string>
            {
                { "client_id", ClientIdFacebook },
                { "client_secret", ClientSecretFacebook },
                { "redirect_uri", redirectUrl },
                { "code", code },
                { "code_verifier", codeVerifier },
                { "grant_type", "authorization_code" }
            };

            var tokenResult = await HttpClientHelper.SendPostRequest<TokenResult>("https://graph.facebook.com/v10.0/oauth/access_token", authParams);
            return tokenResult;
        }
    }
}
