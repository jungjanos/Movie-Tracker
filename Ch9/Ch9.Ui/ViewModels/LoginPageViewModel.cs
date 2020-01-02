﻿using Ch9.Services;
using Ch9.Infrastructure.Extensions;
using Ch9.Models;
using Ch9.Services.Contracts;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Ch9.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        private readonly ISettings _settings;
        private readonly ITmdbApiService _tmdbApiService;

        private string _userName;
        public string UserName
        {
            get => _userName;
            set => SetProperty(ref _userName, value);
        }

        private string _password;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public bool HideLoginPageFlag { get; set; }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand SubmitCommand { get; private set; }
        public ICommand SignUpCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        public LoginPageViewModel(
            ISettings settings,
            ITmdbApiService tmdbApiService,
            IPageService pageService,
            string username = null,
            string password = null
            ) : base(pageService)
        {
            _settings = settings;
            _tmdbApiService = tmdbApiService;
            UserName = username;
            Password = password;
            HideLoginPageFlag = _settings.IsLoginPageDeactivationRequested;

            SubmitCommand = new Command(async () => await OnSubmit());

            SignUpCommand = new Command(async () => await _pageService.OpenWeblink("https://www.themoviedb.org/account/signup"));
            CancelCommand = new Command(async () =>
            {
                _settings.IsLoginPageDeactivationRequested = HideLoginPageFlag;
                await _settings.SavePropertiesAsync();
                await _pageService.PopCurrent();
            });
        }

        private async Task OnSubmit(int retryCount = 1, int delayMilliseconds = 1000)
        {
            if (IsBusy)
                return;

            var userName = UserName;
            var password = Password;

            var syntacticCheckResult = ValidateCredentialSyntactics(userName, password);
            if (!string.IsNullOrEmpty(syntacticCheckResult))
            {
                ErrorMessage = syntacticCheckResult;
                return;
            }

            ErrorMessage = string.Empty;
            IsBusy = true;

            var result = await TryTmdbSignin(userName, password, retryCount, delayMilliseconds);

            if (result.Success)
            {
                _settings.SessionId = result.NewSessionId;

                _settings.AccountName = userName;
                _settings.Password = password;
                _settings.IsLoginPageDeactivationRequested = true;
                await _settings.SavePropertiesAsync();
            }
            else
            {
                _settings.SessionId = null;
                await _settings.SavePropertiesAsync();
                ErrorMessage = "Login failure";
            }
            IsBusy = false;

            if (_settings.IsLoggedin)
                await _pageService.PopCurrent();
        }

        // Tries to generate a new SessionId for the account
        // Tries to dispose any previous SessionId if available (best effort)
        // Returns bool: success status and the new string: Sessionid
        //
        // CALLER IS RESPONSIBLE TO SET Settings.SessionId according to the result.
        // Results CAN NOT be ignored (side effect on server)
        private async Task<(bool Success, string NewSessionId)> TryTmdbSignin(string accountName, string password, int retryCount, int delayMilliseconds)
        {
            string nullStr = null;
            var result = (Success: false, NewSessionId: nullStr);

            SessionIdResponseModel newSession = null;

            try
            {
                var response = await _tmdbApiService.TryCreateRequestToken(retryCount, delayMilliseconds);

                if (!response.HttpStatusCode.IsSuccessCode())
                {
                    await _pageService.DisplayAlert("Sign in error", $"Error getting request token from TMDB server, server response: {response.HttpStatusCode}", "Ok");
                    return result;
                }

                var token = response.RequestToken;
                if (!token.Success)
                {
                    await _pageService.DisplayAlert("Sign in error", "TMDB server indicated failure in request token", "Ok");
                    return result;
                }

                var response2 = await _tmdbApiService.TryValidateRequestTokenWithLogin(accountName, password, token.Token, retryCount, delayMilliseconds);

                if (!response2.HttpStatusCode.IsSuccessCode())
                {
                    await _pageService.DisplayAlert("Sign in error", $"TMDB server reported error when authenticating with supplied account credentials, server response: {response2.HttpStatusCode}", "Ok");
                    return result;
                }

                string validatedToken = response2.RequestToken.Token;

                var response3 = await _tmdbApiService.TryCreateSessionId(validatedToken, retryCount, delayMilliseconds);

                if (!response3.HttpStatusCode.IsSuccessCode())
                {
                    await _pageService.DisplayAlert("Sign in error", $"TMDB server reported error when creating a new session id, server response: {response3.HttpStatusCode}", "Ok");
                    return result;
                }

                newSession = response3.SessionIdResponseModel;
            }
            catch (Exception ex)
            {
                await _pageService.DisplayAlert("Sign in error", $"Exception was thrown during sign in procedure. Details: {ex.Message}", "Ok");
                return result;
            }

            result.Success = !string.IsNullOrWhiteSpace(newSession?.SessionId);
            result.NewSessionId = newSession?.SessionId;

            return result;
        }

        private string ValidateCredentialSyntactics(string userName, string password)
        {
            List<string> errorMessages = new List<string>();

            if (string.IsNullOrWhiteSpace(userName))
                errorMessages.Add("username can not be empty");
            if (string.IsNullOrWhiteSpace(password))
                errorMessages.Add("password can not be empty");

            return string.Join(", ", errorMessages);
        }
    }
}