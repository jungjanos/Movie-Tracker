using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Ch9.Services;

namespace Ch9.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        private bool _runOnlyOnce;
        private int _numberOfTimeInitializationRun = 0;
        private Func<Task> _initializationAction = null;

        protected readonly IPageService _pageService;

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        /// <summary>
        /// Constructor can be injected with null object if no page services will be required
        /// </summary>
        /// <param name="pageService">Object to provide Xamarin.Page related services (navigation, alterts, etc...) to the ViewModel</param>
        protected ViewModelBase(IPageService pageService)
        {
            _pageService = pageService;
        }

        /// <summary>
        /// Set the async initialization action for the ViewModel. 
        /// Only call from ViewModel's constructor
        /// </summary>
        /// <param name="initializationAction">Action to be called</param>        
        internal void ConfigureInitialization(Func<Task> initializationAction, bool runOnlyOnce = false)
        {
            _runOnlyOnce = runOnlyOnce;
            _initializationAction = initializationAction;
        }

        /// <summary>
        /// Method providing async initialization logic for the ViewModel when async void Xamarin.Page.OnAppearing()  
        /// requires. Strictly single threaded.         
        /// Throws if not initialization was not configured beforehand
        /// </summary>        
        public virtual async Task Initialize()
        {
            if (_numberOfTimeInitializationRun == 0)
            {
                await _initializationAction.Invoke();
                ++_numberOfTimeInitializationRun;
            }
            else if (!_runOnlyOnce)
            {
                await _initializationAction.Invoke();
                ++_numberOfTimeInitializationRun;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
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
