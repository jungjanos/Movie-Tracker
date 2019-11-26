using Ch9.Services;
using Ch9.Ui.Contracts.Models;
using Ch9.Services.Contracts;
using Ch9.Utils;
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
        private readonly ISettings _settings;        
        private readonly ITmdbApiService _tmdbApiService;
        private readonly IMovieDetailModelConfigurator _movieDetailModelConfigurator;
        private readonly IPersonDetailModelConfigurator _personDetailModelConfigurator;
        private readonly WeblinkComposer _weblinkComposer;
        private readonly Task _fetchGalleryImages;
        private readonly Task _fetchPersonsMovieCredits;

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
            ISettings settings,            
            ITmdbApiService tmdbApiService,
            IMovieDetailModelConfigurator movieDetailModelConfigurator,
            IPersonDetailModelConfigurator personDetailModelConfigurator,
            WeblinkComposer weblinkComposer,
            IPageService pageService) : base(pageService)
        {
            _settings = settings;            
            _tmdbApiService = tmdbApiService;
            _movieDetailModelConfigurator = movieDetailModelConfigurator;
            _personDetailModelConfigurator = personDetailModelConfigurator;
            _weblinkComposer = weblinkComposer;

            PersonsDetails = personDetails;

            var firstImage = new ImageModel();
            _personDetailModelConfigurator.SetProfileGalleryPictureImageSrc(firstImage, PersonsDetails);

            DisplayImages = new ObservableCollection<ImageModel>(new ImageModel[] { firstImage });

            OnItemTappedCommand = new Command<Ui.Contracts.Models.MovieDetailModel>(async mov => await _pageService.PushAsync(mov));

            OpenWeblinkCommand = new Command<string>(async url => await _pageService.OpenWeblink(url));
            OpenInfolinkCommand = new Command(async () =>
            {
                string weblink = _weblinkComposer.Compose(PersonsDetails);

                if (!string.IsNullOrEmpty(weblink))
                    await _pageService.OpenWeblink(weblink);
            });

            _fetchGalleryImages = UpdateImageCollection();
            _fetchPersonsMovieCredits = _fetchGalleryImages.ContinueWith(async t => await FetchPersonsMovieCredits(), cancellationToken: System.Threading.CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private async Task UpdateImageCollection()
        {
            IsBusy = true;
            try
            {
                var response = await _tmdbApiService.TryGetPersonImages(PersonsDetails.Id, retryCount: 1, delayMilliseconds: 1000, fromCache: true);

                if (response.HttpStatusCode.IsSuccessCode())
                {
                    ImageModel[] imagesFetched = response.Images;
                    _personDetailModelConfigurator.SetProfileGalleryImageSources(imagesFetched);

                    var first = DisplayImages.FirstOrDefault();

                    foreach (var image in imagesFetched)
                        if (image.FilePath != first?.FilePath)
                            DisplayImages.Add(image);
                }
            }
            catch (Exception ex) { await _pageService.DisplayAlert("Error", $"Could not load gallery, service responded with: {ex.Message}", "Ok"); }
            finally { IsBusy = false; }
        }

        private async Task FetchPersonsMovieCredits()
        {
            IsBusy = true;
            try
            {
                var response = await _tmdbApiService.TryGetPersonsMovieCredits(PersonsDetails.Id, _settings.SearchLanguage, 1, 1000, fromCache: true);
                if (response.HttpStatusCode.IsSuccessCode())
                {
                    var personsMovieCredits = response.PersonsMovieCreditsModel;

                    await Task.Run(() =>
                    {
                        _movieDetailModelConfigurator.SetImageSrc(personsMovieCredits.MoviesAsActor);
                        _movieDetailModelConfigurator.SetImageSrc(personsMovieCredits.MoviesAsCrewMember);
                        _movieDetailModelConfigurator.SetGenreNamesFromGenreIds(personsMovieCredits.MoviesAsActor);
                        _movieDetailModelConfigurator.SetGenreNamesFromGenreIds(personsMovieCredits.MoviesAsCrewMember);
                        personsMovieCredits.SortMoviesByYearDesc();
                    });
                    PersonsMovieCreditsModel = personsMovieCredits;
                }
                else
                    await _pageService.DisplayAlert("Error", $"Could not fetch persons movie participations, service responded: FetchPersonsMovieCreditsDetails():{response.HttpStatusCode}", "Ok");
            }
            catch (Exception ex) { await _pageService.DisplayAlert("Error", $"Could not fetch persons movie participations, exception at: FetchPersonsMovieCreditsDetails():{ex.Message}", "Ok"); }
            finally { IsBusy = false; }
        }
    }
}
