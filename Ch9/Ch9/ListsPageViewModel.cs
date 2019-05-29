using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Ch9.Models;

namespace Ch9
{
    // Remark:
    // Up to this point in development I didnt use MVVM 
    // At this point the extreme slowness of the Android emulator and 
    // the annoying debugging of Xamarin related pieces 
    // Combined with my inexperience of XAML has forced me 
    // to make a clearer separation of Xamarin page display (framework) from page behavior
    // to be able to develop and test page behavior separatelly from Xamarin framework as POCO
    // classes
    // The goal is to make the development more rapid and not to achieve MVVM purism
    class ListsPageViewModel : INotifyPropertyChanged
    {
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
