using Ch9.Services;
using Ch9.ViewModels;
using Ch9.Ui;

using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Autofac;

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
            using (var scope = DependencyResolver.Container.BeginLifetimeScope())
            {
                ViewModel = scope.Resolve<MainSettingsPage2ViewModel>(new TypedParameter[] { new TypedParameter(typeof(IPageService), new PageService(this)) });
            }

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

        private void OnSelectGenres_Tapped(object sender, EventArgs e)
        {
            //await Navigation.PushAsync(new GenreSettingsPage());
            ViewModel.OpenMovieGenreSelectionCommand.Execute(null);

        }


    }
}