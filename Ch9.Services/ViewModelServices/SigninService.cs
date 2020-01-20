using Ch9.Infrastructure.Extensions;
using Ch9.Services.Contracts;

using System;
using System.Threading.Tasks;

namespace Ch9.Services.ViewModelServices
{
    public class SigninService : ISigninService
    {
        private readonly ISettings _settings;
        private readonly ITmdbApiService _tmdbApiService;

        public SigninService(ISettings settings, ITmdbApiService tmdbApiService)
        {
            _settings = settings;
            _tmdbApiService = tmdbApiService;
        }

        /// <summary>
        /// Tries to generate a new SessionId for the account
        /// Tries to dispose any previous SessionId if available (best effort)
        /// </summary>   
        public async Task Signin(string userName, string password, int retryCount, int delayMilliseconds)
        {
            var response = await _tmdbApiService.TryCreateRequestToken(retryCount, delayMilliseconds);
            if (!response.HttpStatusCode.IsSuccessCode())
                throw new Exception("$Error getting request token from TMDB server, server response: {response.HttpStatusCode}");

            var token = response.RequestToken;
            if (!token.Success)
                throw new Exception("TMDB server indicated failure in request token");

            var response2 = await _tmdbApiService.TryValidateRequestTokenWithLogin(userName, password, token.Token, retryCount, delayMilliseconds);
            if (!response2.HttpStatusCode.IsSuccessCode())
                throw new Exception($"TMDB server reported error when authenticating with supplied account credentials, server response: {response2.HttpStatusCode}");

            string validatedToken = response2.RequestToken.Token;

            var response3 = await _tmdbApiService.TryCreateSessionId(validatedToken, retryCount, delayMilliseconds);

            if (!response3.HttpStatusCode.IsSuccessCode())
                throw new Exception($"TMDB server reported error when creating a new session id, server response: {response3.HttpStatusCode}");

            var newSession = response3.SessionIdResponseModel;

            bool success = !string.IsNullOrWhiteSpace(newSession?.SessionId);

            if (success)
            {
                _settings.SessionId = newSession?.SessionId;

                _settings.AccountName = userName;
                _settings.Password = password;
                _settings.IsLoginPageDeactivationRequested = true;
                await _settings.SavePropertiesAsync();
            }
            else
            {
                _settings.SessionId = null;
                await _settings.SavePropertiesAsync();

                throw new Exception("Login failure");
            }
        }
    }
}
