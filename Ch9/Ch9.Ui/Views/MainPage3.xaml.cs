using Ch9.Services;
using Ch9.Ui;
using Ch9.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Autofac;
using Autofac.Util;
using Autofac.Core.Resolving;
using Ch9.Services.Contracts;
using Ch9.Models;
using Ch9.Data.Contracts;
using System.Collections.Generic;
using Ch9.Services.VideoService;

namespace Ch9.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage3 : ContentPage
    {
        public MainPage3ViewModel ViewModel
        {
            get => BindingContext as MainPage3ViewModel;
            set => BindingContext = value;
        }
        public MainPage3()
        {
            InitializeComponent();

            var x = DependencyResolver.Container.Resolve<MainPage3ViewModel>(new TypedParameter[] { new TypedParameter(typeof(IPageService), new PageService(this))});

            ViewModel = x;
        }
    }
}