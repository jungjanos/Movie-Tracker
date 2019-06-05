using Ch9.Models;
using Ch9.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ch9
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MovieDetailPage2 : ContentPage
    {
        public MovieDetailPageViewModel ViewModel
        {
            get => BindingContext as MovieDetailPageViewModel;
            set => BindingContext = value;
        }

        Task vMInitializer;

        public MovieDetailPage2(MovieDetailModel movie)
        {
            ViewModel = new MovieDetailPageViewModel(
                movie,
                ((App)Application.Current).Settings,
                ((App)Application.Current).CachedSearchClient,
                ((App)Application.Current).MovieDetailModelConfigurator,
                new PageService(this)
                );
            vMInitializer = ViewModel.Initialize();

            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await vMInitializer;
        }
    }
}