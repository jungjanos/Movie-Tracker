using Ch9.Models;
using Ch9.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Ch9
{
    public class LargeImagePageViewModel : INotifyPropertyChanged
    {
        public MovieDetailModel Movie { get; }
        private readonly ISettings _settings;
        private readonly IPageService _pageService;

        public LargeImagePageViewModel(
                MovieDetailModel movie,
                ISettings settings,                
                IPageService pageService
            )
        {
            Movie = movie;
            _settings = settings;
            _pageService = pageService;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName]string propertyName = null)
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
