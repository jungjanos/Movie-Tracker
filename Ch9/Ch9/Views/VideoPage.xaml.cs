﻿using Ch9.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ch9.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VideoPage : ContentPage
    {
        public VideoPage(ImageModel videoThumbnailWithVideo)
        {           
            BindingContext = videoThumbnailWithVideo;
            InitializeComponent();
        }
    }
}