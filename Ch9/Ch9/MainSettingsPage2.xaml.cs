using Ch9.Utils;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ch9
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
                ((App)Application.Current).TmdbNetworkClient,
                new PageService(this)
                );

            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            ViewModel.InitializeOnAppearing();
            base.OnAppearing();
        }

        protected override async void OnDisappearing()
        {
            await ViewModel.ValidateOnDisappearing();
            base.OnDisappearing();
        }


        private void OnSearchLanguage_Changed(object sender, EventArgs e)
        {
            ViewModel.SearchLanguageChangedCommand.Execute(null);
        }

        private void OnSelectGenres_Tapped(object sender, EventArgs e)
        {

        }
    }
}