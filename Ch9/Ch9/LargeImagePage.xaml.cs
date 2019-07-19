﻿using Ch9.Models;
using Ch9.Utils;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ch9
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LargeImagePage : ContentPage
    {
        public LargeImagePageViewModel ViewModel
        {
            get => BindingContext as LargeImagePageViewModel;
            set => BindingContext = value;
        }

        public LargeImagePage(MovieDetailModel movie)
        {
            ViewModel = new LargeImagePageViewModel(
                movie,
                ((App)Application.Current).Settings,
                new PageService(this)
                );

            InitializeComponent();
        }
    }
}