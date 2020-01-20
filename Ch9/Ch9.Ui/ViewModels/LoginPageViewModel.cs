using Ch9.Services;
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
        private readonly ISigninService _signinService;

        #region BINDING_PROPERTIES
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
        #endregion

        public ICommand SubmitCommand { get; private set; }
        public ICommand SignUpCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        public LoginPageViewModel(
            ISettings settings,            
            ISigninService signinService,
            IPageService pageService,
            string username = null,
            string password = null
            ) : base(pageService)
        {
            _settings = settings;            
            _signinService = signinService;
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

            var syntaxCheckResult = ValidateCredentialSyntax(userName, password);
            if (!string.IsNullOrEmpty(syntaxCheckResult))
            {
                ErrorMessage = syntaxCheckResult;
                return;
            }

            ErrorMessage = string.Empty;
            IsBusy = true;

            try
            {
                await _signinService.Signin(userName, password, retryCount, delayMilliseconds);
            }
            catch (Exception ex)
            {
                await _pageService.DisplayAlert("Sign in error", ex.Message, "Ok");
            }
            finally
            { IsBusy = false; }

            if (_settings.IsLoggedin)
                await _pageService.PopCurrent();
        }

        private string ValidateCredentialSyntax(string userName, string password)
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
