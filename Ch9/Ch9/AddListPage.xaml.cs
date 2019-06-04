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
    public partial class AddListPage : ContentPage
    {
        public AddListPageViewModel ViewModel {
            get => BindingContext as AddListPageViewModel;
            private set => BindingContext = value;
        }        

        public AddListPage(AddListPageViewModel viewModel)
        {
            viewModel.PageService = new PageService(this);
            ViewModel = viewModel;
            InitializeComponent();
        }        
    }
}