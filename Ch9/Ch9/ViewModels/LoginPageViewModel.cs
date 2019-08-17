using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;

namespace Ch9.ViewModels
{
    public class LoginPageViewModel : INotifyPropertyChanged
    {
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

        private bool _ignoreLoginFlag;
        public bool IgnoreLoginFlag
        {
            get => _ignoreLoginFlag;
            set => SetProperty(ref _ignoreLoginFlag, value);
        }

        public ICommand SubmitCommand { get; private set; } 
        public ICommand SignUpCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }


        public LoginPageViewModel()
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName]string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
