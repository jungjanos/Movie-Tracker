using Ch9.Services;

using System.Windows.Input;
using Xamarin.Forms;

namespace Ch9.ViewModels
{
    public class AddListPageViewModel : ViewModelBase
    {
        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string _description;
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public bool Accepted { get; private set; }

        private readonly ListsPageViewModel3 _prevoiusPageViewModel;

        public ICommand OkCommand { get; set; }

        public AddListPageViewModel(ListsPageViewModel3 previousPageViewModel, IPageService pageService) : base(pageService)
        {
            _prevoiusPageViewModel = previousPageViewModel;

            Description = string.Empty;
            Accepted = false;
            OkCommand = new Command(async () =>
            {
                await _pageService.PopCurrent();
                await _prevoiusPageViewModel.AddList(this);
            });
        }
    }
}
