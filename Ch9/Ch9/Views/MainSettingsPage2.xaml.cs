using Ch9.Services;
using Ch9.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ch9.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainSettingsPage2 : ContentPage
    {
        public MainSettingsPage2ViewModel ViewModel
        {
            get => BindingContext as MainSettingsPage2ViewModel;
            set => BindingContext = value;
        }


        public MainSettingsPage2()
        {
            ViewModel = new MainSettingsPage2ViewModel(
                ((App)Application.Current).Settings,
                ((App)Application.Current).MovieGenreSettings,
                ((App)Application.Current).TmdbApiService,
                new PageService(this)
                );

            InitializeComponent();
        }

        protected override async void OnDisappearing()
        {
            await ViewModel.SaveChanges();
            base.OnDisappearing();
        }


        private void OnSearchLanguage_Changed(object sender, EventArgs e)
        {
            ViewModel.SearchLanguageChangedCommand.Execute(null);
        }

        private async void OnSelectGenres_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new GenreSettingsPage());
        }
    }
}