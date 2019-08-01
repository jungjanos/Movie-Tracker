using Ch9.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Ch9.ViewModels
{
    public class AddListPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        private string _description;
        public IPageService PageService { get; set; }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        } 

        public bool Accepted { get; private set; }


        private readonly ListsPageViewModel3 _prevoiusPageViewModel3;


        public ICommand OkCommand { get; set; }

        public AddListPageViewModel(ListsPageViewModel3 previousPageViewModel3)
        {
            _prevoiusPageViewModel3 = previousPageViewModel3;
            Description = string.Empty;
            Accepted = false;
            OkCommand = new Command(async () =>
            {
                await PageService.PopCurrent();
                await _prevoiusPageViewModel3.AddList(this);
            });
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
