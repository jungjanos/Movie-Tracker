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
    public partial class MainPage2 : ContentPage
    {
        public MainPage2ViewModel ViewModel
        {
            get => BindingContext as MainPage2ViewModel;
            set => BindingContext = value;
        }

        public MainPage2()
        {
            InitializeComponent();
        }
    }
}