using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.PeopleService.v1;
using Google.Apis.PeopleService.v1.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OAuthTutorial.Helpers;
using OAuthTutorial.Services;
using System;
using System.Threading.Tasks;
using System.Linq;
using Facebook;

namespace OAuthTutorial.Controllers
{
    public class GoogleOAuthController : Controller
    {
        private const string RedirectUrl = "https://localhost:60419/GoogleOAuth/Code";
        private const string RedirectUrlFacebook = "https://localhost:60419/GoogleOAuth/CodeFacebook";

        //private const string Scope = "openid";
        //private const string Scope = "profile";
        private const string Scope = "email profile";
        private const string PkceSessionKey = "codeVerifier";

        private const string ClientId = "197979692814-oio76eojdgt4v5rtmfapfvre19752cgf.apps.googleusercontent.com";
        private const string ClientSecret = "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";

        private const string ClientIdFacebook = "968685937912559";
        private const string ClientSecretFacebook = "76fb5d4c823e42b88132478de96fe02f";

        private const string PkceSessionKeyFacebook = "codeVerifierFacebook";

        private const string OAuthServerEndpoint = "https://accounts.google.com/o/oauth2/v2/auth";
        private const string TokenServerEndpoint = "https://oauth2.googleapis.com/token";

        public IActionResult RedirectOnOAuthServer()
        {          
            // PCKE.
           var codeVerifier = Guid.NewGuid().ToString();
            var codeChellange = Sha256Helper.ComputeHash(codeVerifier);

            HttpContext.Session.SetString(PkceSessionKey, codeVerifier);

            var url = GoogleOAuthService.GenerateOAuthRequestUrl(Scope, RedirectUrl, codeChellange);
            return Redirect(url);
        }

        public async Task<IActionResult> CodeAsync(string code)
        {
            var codeVerifier = HttpContext.Session.GetString(PkceSessionKey);

            var tokenResult = await GoogleOAuthService.ExchangeCodeOnTokenAsync(code, codeVerifier, RedirectUrl);

            GoogleCredential credential = GoogleCredential.FromAccessToken(tokenResult.AccessToken);

            PeopleServiceService service = new(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                //ApplicationName = "TestOuath",
            });

            PeopleResource.GetRequest profileRequest = service.People.Get("people/me");
            profileRequest.PersonFields = "EmailAddresses";
            //profileRequest.PersonFields = "Names";

            Person profile = profileRequest.Execute();
            string email = profile.EmailAddresses.FirstOrDefault()?.Value;            

            return Ok(email);
        }

        public IActionResult Facebook()
        {
            var codeVerifier = Guid.NewGuid().ToString() + Guid.NewGuid().ToString();
            var codeChellange = Sha256Helper.ComputeHash(codeVerifier);

            HttpContext.Session.SetString(PkceSessionKeyFacebook, codeVerifier);

            var url = GoogleOAuthService.GenerateOAuthRequestUrlFacebook(RedirectUrlFacebook, codeChellange);
            return Redirect(url);
        }

        public async Task<IActionResult> CodeFacebookAsync(string code)
        {
            var codeVerifier = HttpContext.Session.GetString(PkceSessionKeyFacebook);

            var tokenResult = await GoogleOAuthService.ExchangeCodeOnTokenFacebookAsync(code, codeVerifier, RedirectUrlFacebook);

            var facebookClient = new FacebookClient(tokenResult.AccessToken);
            dynamic result = facebookClient.Get("me?fields=id,name,email");
            string email = result.email;

            return Ok(email);
        }

    }
}

