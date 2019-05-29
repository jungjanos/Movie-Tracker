using Ch9.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ch9
{
    public class DisplayedMovieList : ObservableCollection<MovieModel>
    {
        private event PropertyChangedEventHandler PropertyChanged;
        private MovieListModel movieList;
        private bool expanded;
        public bool Expanded
        {
            get => expanded;
            set => SetProperty(ref expanded, value);
        }

        public DisplayedMovieList(MovieListModel movieList, bool expanded)
        {
            this.movieList = movieList;
            this.expanded = expanded;
        }

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
        public string StateIcon
        {
            get { return Expanded ? "expanded_blue.png" : "collapsed_blue.png"; }
        }
    }




    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListsPage : ContentPage
    {
        public ObservableCollection<MovieListModel> availableLists;
        private MovieListModel currentlyDisplayedList;
        private ObservableCollection<MovieModel> displayedMovies;
        private Task<Task> fetchUiData;

        public string[] ListNames => availableLists.Select(x => x.Name).ToArray();
               
        public ListsPage()
        {

            availableLists = new ObservableCollection<MovieListModel>();

            fetchUiData = ((App)Application.Current).UserLists.ContinueWith(PrepareDataForUI);

            InitializeComponent();

            async Task PrepareDataForUI(Task<MovieListModel[]> listsTask)
            {
                var lists = await listsTask;
                foreach (var list in lists)
                    availableLists.Add(list);

                currentlyDisplayedList = availableLists.FirstOrDefault();

                if (currentlyDisplayedList == null)
                    displayedMovies = new ObservableCollection<MovieModel>();
                else
                    displayedMovies = new ObservableCollection<MovieModel>(currentlyDisplayedList.Movies);

                BindingContext = this.availableLists;
                //viewedListPicker.SetBinding(Picker.ItemsSourceProperty, "availableLists");
                //viewedListPicker.ItemDisplayBinding = new Binding("Name");
                //BindingContext = this;
                
                //viewedListPicker.ItemsSource = new string[] { "aaa", "bbb", "ccc" };
            }
        }

        protected override async void OnAppearing()
        {
            await await fetchUiData;
            label.Text = availableLists.Count + availableLists.First().Name;
            base.OnAppearing();
        }


    }
}