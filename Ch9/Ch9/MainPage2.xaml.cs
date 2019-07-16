using Ch9.Utils;
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
            ViewModel = new MainPage2ViewModel(
                ((App)Application.Current).Settings,
                ((App)Application.Current).CachedSearchClient,
                ((App)Application.Current).MovieDetailModelConfigurator,
                new PageService(this));
        }

        private void SearchResult_ItemTapped(object sender, ItemTappedEventArgs e) => ViewModel.ItemTappedCommand.Execute(e.Item);

    }    
}