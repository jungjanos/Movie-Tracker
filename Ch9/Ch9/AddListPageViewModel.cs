using Ch9.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Ch9
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

        // TODO remove this
        private readonly ListsPageViewModel _prevoiusPageViewModel;
        private readonly ListsPageViewModel2 _prevoiusPageViewModel2;


        public ICommand OkCommand { get; set; }

        // TODO remove this
        public AddListPageViewModel(ListsPageViewModel previousPageViewModel)
        {
            _prevoiusPageViewModel = previousPageViewModel;
            Description = string.Empty;
            Accepted = false;            
            OkCommand = new Command(async () => 
            {
                await PageService.PopCurrent();
                await _prevoiusPageViewModel.AddList(this);
            });
        }

        public AddListPageViewModel(ListsPageViewModel2 previousPageViewModel2)
        {
            _prevoiusPageViewModel2 = previousPageViewModel2;
            Description = string.Empty;
            Accepted = false;
            OkCommand = new Command(async () =>
            {
                await PageService.PopCurrent();
                await _prevoiusPageViewModel2.AddList(this);
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
