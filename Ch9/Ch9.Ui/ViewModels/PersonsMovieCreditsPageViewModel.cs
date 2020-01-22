using Ch9.Services;
using Ch9.Models;
using Ch9.Services.Contracts;

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Ch9.ViewModels
{
    public class PersonsMovieCreditsPageViewModel : ViewModelBase
    {
        private readonly IPersonDetailModelConfigurator _personDetailModelConfigurator;
        private readonly IActorDetailService _actorDetailService;
        private readonly IWeblinkComposer _weblinkComposer;

        public PersonsDetailsModel PersonsDetails { get; private set; }
        private PersonsMovieCreditsModel _personsMovieCreditsModel;
        public PersonsMovieCreditsModel PersonsMovieCreditsModel
        {
            get => _personsMovieCreditsModel;
            set
            {
                if (SetProperty(ref _personsMovieCreditsModel, value))
                {
                    OnPropertyChanged(nameof(NumberOfMoviesInSelectedMovieCreditType));
                    OnPropertyChanged(nameof(NumberOfMoviesAsActor));
                    OnPropertyChanged(nameof(NumberOfMoviesAsCrew));
                }
            }
        }
        public ObservableCollection<ImageModel> DisplayImages { get; private set; }
        public int NumberOfMoviesInSelectedMovieCreditType => _actorOrCrewSwitch ? NumberOfMoviesAsCrew : NumberOfMoviesAsActor;
        public int NumberOfMoviesAsActor => PersonsMovieCreditsModel?.MoviesAsActor?.Length ?? 0;
        public int NumberOfMoviesAsCrew => PersonsMovieCreditsModel?.MoviesAsCrewMember?.Length ?? 0;

        private bool _actorOrCrewSwitch;
        public bool ActorOrCrewSwitch
        {
            get => _actorOrCrewSwitch;
            set
            {
                if (SetProperty(ref _actorOrCrewSwitch, value))
                    OnPropertyChanged(nameof(NumberOfMoviesInSelectedMovieCreditType));
            }
        }

        public ICommand OnItemTappedCommand { get; private set; }
        public ICommand OpenWeblinkCommand { get; private set; }
        public ICommand OpenInfolinkCommand { get; private set; }

        public PersonsMovieCreditsPageViewModel(
            PersonsDetailsModel personDetails,
            IPersonDetailModelConfigurator personDetailModelConfigurator,
            IActorDetailService actorDetailService,
            IWeblinkComposer weblinkComposer,
            IPageService pageService) : base(pageService)
        {
            _personDetailModelConfigurator = personDetailModelConfigurator;
            _actorDetailService = actorDetailService;
            _weblinkComposer = weblinkComposer;

            PersonsDetails = personDetails;

            var firstImage = new ImageModel();
            _personDetailModelConfigurator.SetProfileGalleryPictureImageSrc(firstImage, PersonsDetails);

            DisplayImages = new ObservableCollection<ImageModel>(new ImageModel[] { firstImage });

            OnItemTappedCommand = new Command<MovieDetailModel>(async mov => await _pageService.PushAsync(mov));

            OpenWeblinkCommand = new Command<string>(async url => await _pageService.OpenWeblink(url));
            OpenInfolinkCommand = new Command(async () =>
            {
                string weblink = _weblinkComposer.Compose(PersonsDetails);

                if (!string.IsNullOrEmpty(weblink))
                    await _pageService.OpenWeblink(weblink);
            });            

            ConfigureInitialization(async () =>  { await LoadImageCollection().ContinueWith(async t => await LoadPersonsMovieCredits()); }, runOnlyOnce: true);
        }

        private async Task LoadImageCollection()
        {
            IsBusy = true;
            try
            {
                var gallery = await _actorDetailService.LoadPersonsImageCollection(PersonsDetails.Id, retryCount: 1, delayMilliseconds: 1000, fromCahe: true);

                // first image might have a duplacate in the gallery, we should not include it a second time
                foreach (var image in gallery.Where(img => img.FilePath != DisplayImages.FirstOrDefault()?.FilePath))
                    DisplayImages.Add(image);
            }
            catch (Exception ex) { await _pageService.DisplayAlert("Error", $"Could not load gallery, service responded with: {ex.Message}", "Ok"); }
            finally { IsBusy = false; }
        }

        private async Task LoadPersonsMovieCredits()
        {
            IsBusy = true;
            try
            {
                var response = await _actorDetailService.LoadPersonsMovieCredits(PersonsDetails.Id, retryCount: 1, delayMilliseconds: 1000, fromCahe: true);
                PersonsMovieCreditsModel = response;
            }
            catch (Exception ex) { await _pageService.DisplayAlert("Error", ex.Message, "Ok"); }
            finally { IsBusy = false; }
        }
    }
}
