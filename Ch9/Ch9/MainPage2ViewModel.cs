using Ch9.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Ch9
{
    public class MainPage2ViewModel : INotifyPropertyChanged
    {        
        public event PropertyChangedEventHandler PropertyChanged;

        private string _searchString;
        public string SearchString
        {
            get => _searchString;
            set => SetProperty(ref _searchString, value);
        }

        private MovieDetailModel _searchResults;
        public MovieDetailModel SearchResults
        {
            get => _searchResults;
            set => SetProperty(ref _searchResults, value);
        }



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
