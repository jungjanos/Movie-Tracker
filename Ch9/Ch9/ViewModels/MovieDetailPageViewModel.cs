using Ch9.ApiClient;
using Ch9.Services;
using Ch9.Services.VideoService;
using Ch9.Ui.Contracts.Models;
using Ch9.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using GetPersonsDetailsModel = Ch9.Models.GetPersonsDetailsModel;

namespace Ch9.ViewModels
{
    public class MovieDetailPageViewModel : ViewModelBase
    {
        public MovieDetailModel Movie { get; set; }
        private readonly ISettings _settings;
        private readonly ITmdbCachedSearchClient _cachedSearchClient;
        private readonly UsersMovieListsService2 _movieListsService2;
        private readonly IMovieDetailModelConfigurator _movieDetailModelConfigurator;
        private readonly IPersonDetailModelConfigurator _personDetailModelConfigurator;
        private readonly IVideoService _videoService;
        private readonly WeblinkComposer _weblinkComposer;
        private readonly Task _fetchGallery;
        private MovieCreditsModel _credits;

        private ObservableCollection<ImageModel> _displayImages;
        public ObservableCollection<ImageModel> DisplayImages
        {
            get => _displayImages;
            set => SetProperty(ref _displayImages, value);
        }

        public ImageModel SelectedGalleryImage { get; set; }

        private bool _galleryIsBusy;
        public bool GalleryIsBusy
        {
            get => _galleryIsBusy;
            set => SetProperty(ref _galleryIsBusy, value);
        }

        private bool _waitingOnVideo;
        public bool WaitingOnVideo
        {
            get => _waitingOnVideo;
            set => SetProperty(ref _waitingOnVideo, value);
        }

        public bool? MovieIsAlreadyOnActiveList => _movieListsService2.CustomListsService.CheckIfMovieIsOnActiveList(Movie.Id);

        private AccountMovieStates _movieStates;
        public AccountMovieStates MovieStates
        {
            get => _movieStates;
            set => SetProperty(ref _movieStates, value);
        }

        private bool _displayImageTypeSelector;
        /// <summary>
        /// tracks the type of images displayed on the View. 
        /// false : displays simple images, true : displays video thumbnails
        /// </summary>
        public bool DisplayImageTypeSelector
        {
            get => _displayImageTypeSelector;
            set => SetProperty(ref _displayImageTypeSelector, value);
        }

        private bool _showCredits;
        public bool ShowCredits
        {
            get => _showCredits;
            set => SetProperty(ref _showCredits, value);
        }

        private List<IStaffMemberRole> _staffs;
        public List<IStaffMemberRole> Staffs
        {
            get => _staffs;
            set => SetProperty(ref _staffs, value);
        }

        public ICommand HomeCommand { get; private set; }
        public ICommand RecommendationsCommand { get; private set; }
        public ICommand ReviewsCommand { get; private set; }
        public ICommand ToggleWatchlistCommand { get; private set; }
        public ICommand AddToListCommand { get; private set; }
        public ICommand ToggleFavoriteCommand { get; private set; }
        public ICommand TapImageCommand { get; private set; }
        public ICommand ChangeDisplayedImageTypeCommand { get; private set; }
        public ICommand ToggleCreditsCommand { get; private set; }
        public ICommand MovieCastPersonTappedCommand { get; private set; }
        public ICommand OpenInfolinkCommand { get; private set; }

        public MovieDetailPageViewModel(
            MovieDetailModel movie,
            ISettings settings,
            ITmdbCachedSearchClient cachedSearchClient,
            UsersMovieListsService2 movieListsService2,
            IMovieDetailModelConfigurator movieDetailModelConfigurator,
            IPersonDetailModelConfigurator personDetailModelConfigurator,
            IVideoService videoService,
            WeblinkComposer weblinkComposer,
            IPageService pageService) : base(pageService)
        {
            Movie = movie;
            _settings = settings;
            _cachedSearchClient = cachedSearchClient;
            _movieListsService2 = movieListsService2;
            _movieDetailModelConfigurator = movieDetailModelConfigurator;
            _personDetailModelConfigurator = personDetailModelConfigurator;
            _videoService = videoService;
            _weblinkComposer = weblinkComposer;
            _fetchGallery = UpdateImageCollection();

            HomeCommand = new Command(async () => await _pageService.PopToRootAsync());
            ReviewsCommand = new Command(async () => await _pageService.PushReviewsPage(this));
            RecommendationsCommand = new Command(async () => await _pageService.PushRecommendationsPageAsync(Movie));
            ToggleWatchlistCommand = new Command(async () => await OnToggleWatchlistCommand());
            AddToListCommand = new Command(async () => await OnAddToListCommand());
            ToggleFavoriteCommand = new Command(async () => await OnToggleFavoriteCommand());
            TapImageCommand = new Command(async () =>
            {
                var capture = SelectedGalleryImage;

                if (capture == null)
                    return;

                if (!capture.HasAttachedVideo)
                    await _pageService.PushLargeImagePageAsync(this);
                else
                {
                    if (WaitingOnVideo)
                        return;
                    else
                        WaitingOnVideo = true;

                    await _videoService.PlayVideo(capture.AttachedVideo, _pageService);

                    WaitingOnVideo = false;
                }
            });
            ChangeDisplayedImageTypeCommand = new Command(async () =>
            {
                GalleryIsBusy = true;
                ((Command)ChangeDisplayedImageTypeCommand).ChangeCanExecute();
                if (DisplayImageTypeSelector == false)
                {
                    await _fetchGallery;
                    await UpdateThumbnailCollection();

                    DisplayImages = null;
                    DisplayImages = Movie.VideoThumbnails;
                }
                else
                {
                    DisplayImages = null;
                    DisplayImages = Movie.MovieImages;
                }
                DisplayImageTypeSelector = !DisplayImageTypeSelector;
                SelectedGalleryImage = DisplayImages.FirstOrDefault();
                GalleryIsBusy = false;
                ((Command)ChangeDisplayedImageTypeCommand).ChangeCanExecute();

            }, () => !GalleryIsBusy);
            ToggleCreditsCommand = new Command(async () =>
            {
                if (!ShowCredits)
                    await FetchCredits();
                ShowCredits = !_showCredits;
            });
            MovieCastPersonTappedCommand = new Command<IStaffMemberRole>(async staffMemberRole =>
            {
                await OpenPersonPage(staffMemberRole);
            });
            OpenInfolinkCommand = new Command(async () =>
            {
                string weblink = _weblinkComposer.Compose(Movie);

                if (!string.IsNullOrEmpty(weblink))
                    await _pageService.OpenWeblink(weblink);
            });

            DisplayImages = Movie.MovieImages;

            // Ensures that the custom list service is initialized (to show the Movie's status on the active custom list)
            // Ensures that account movie states have been fetched
            Func<Task> initializationAction = async () =>
            {
                var t = _movieListsService2.CustomListsService.TryEnsureInitialization();

                try
                {
                    GetMovieDetailsWithAccountStatesResult movieDetailsResult = await _cachedSearchClient.GetMovieDetailsWithAccountStates(Movie.Id, _settings.SearchLanguage, retryCount: 1, delayMilliseconds: 1000);

                    if (movieDetailsResult.HttpStatusCode.IsSuccessCode())
                    {
                        JsonConvert.PopulateObject(movieDetailsResult.Json, Movie);
                        MovieStates = movieDetailsResult.ExtractAccountStates();
                    }
                }
                catch (Exception ex)
                { await _pageService.DisplayAlert("Error", $"Could not fetch movie details, exception: {ex.Message}", "Ok"); }

                try
                {
                    await t;
                    OnPropertyChanged(nameof(MovieIsAlreadyOnActiveList));
                }
                catch (Exception ex)
                { await _pageService.DisplayAlert("Error", $" Exception thrown from {nameof(_movieListsService2.CustomListsService)}.{nameof(_movieListsService2.CustomListsService.TryEnsureInitialization)}, message: {ex.Message}", "Ok"); }
            };

            ConfigureInitialization(initializationAction, runOnlyOnce: true);
        }

        // starts a hot task to fetch and adjust gallery image paths as early as possible        
        private async Task UpdateImageCollection()
        {
            GalleryIsBusy = true;
            try
            {
                GetMovieImagesResult response = await _cachedSearchClient.GetMovieImages(Movie.Id, _settings.SearchLanguage, otherLanguage: null, includeLanguageless: true, retryCount: 0, delayMilliseconds: 1000, fromCache: true);

                if (response.HttpStatusCode.IsSuccessCode())
                {
                    ImageDetailCollection imageCollection = null;
                    await Task.Run(() =>
                    {
                        imageCollection = JsonConvert.DeserializeObject<ImageDetailCollection>(response.Json);
                    });
                    Movie.ImageDetailCollection = imageCollection;
                    // TODO : refactor SetGalleryImageSources() to make it background friendly
                    _movieDetailModelConfigurator.SetGalleryImageSources(Movie);
                }
            }
            catch (Exception ex) { await _pageService.DisplayAlert("Error", $"Could not load gallery, service responded with: {ex.Message}", "Ok"); }
            finally { GalleryIsBusy = false; }
        }
        public async Task UpdateThumbnailCollection()
        {
            if (Movie.VideoThumbnails != null)
                return;

            Movie.VideoThumbnails = new ObservableCollection<ImageModel>();

            try
            {
                List<ImageModel> videoThumbnails = await _videoService.GetVideoThumbnails(Movie.Id, 1, 1000, true);
                foreach (var thumbnail in videoThumbnails)
                    Movie.VideoThumbnails.Add(thumbnail);
            }
            catch (Exception ex)
            { await _pageService.DisplayAlert("Error", $"Could not load video thumbnails, service responded with: {ex.Message}", "Ok"); }
        }

        public async Task OnToggleWatchlistCommand()
        {
            if (!_settings.IsLoggedin)
            {
                await _pageService.DisplayAlert("Info", "You have to log in with a user account to use this function", "Ok");
                return;
            }

            if (MovieStates == null)
                return;

            bool desiredState = !MovieStates.OnWatchlist;

            try
            {
                await _movieListsService2.WatchlistService.ToggleWatchlistState(Movie, desiredState);
                MovieStates.OnWatchlist = desiredState;
                OnPropertyChanged(nameof(MovieStates));
            }
            catch (Exception ex)
            { await _pageService.DisplayAlert("Error", $"Could not change watchlist state, service responded with: {ex.Message}", "Ok"); }
        }

        public async Task OnAddToListCommand()
        {
            if (!_settings.IsLoggedin)
            {
                await _pageService.DisplayAlert("Info", "You have to log in with a user account to use this function", "Ok");
                return;
            }

            try
            {
                await _movieListsService2.CustomListsService.TryEnsureInitialization();

                if (MovieIsAlreadyOnActiveList == null)
                {
                    await _pageService.DisplayAlert("Info", "You have to select a valid public list to be able to add movies to it", "Cancel");
                    return;
                }

                if (MovieIsAlreadyOnActiveList == true)
                {
                    await _movieListsService2.CustomListsService.RemoveMovieFromActiveList(Movie.Id);
                    OnPropertyChanged(nameof(MovieIsAlreadyOnActiveList));
                    return;
                }

                if (MovieIsAlreadyOnActiveList == false)
                {
                    await _movieListsService2.CustomListsService.AddMovieToActiveList(Movie);
                    OnPropertyChanged(nameof(MovieIsAlreadyOnActiveList));
                }
            }
            catch (Exception ex) { await _pageService.DisplayAlert("Error", $"Service responded with: {ex.Message}", "Ok"); }
        }

        public async Task OnToggleFavoriteCommand()
        {
            if (!_settings.IsLoggedin)
            {
                await _pageService.DisplayAlert("Info", "You have to log in with a user account to use this function", "Ok");
                return;
            }

            if (MovieStates == null)
                return;

            bool desiredState = !MovieStates.IsFavorite;

            try
            {
                await _movieListsService2.FavoriteMoviesListService.ToggleFavoriteState(Movie, desiredState);
                MovieStates.IsFavorite = desiredState;
                OnPropertyChanged(nameof(MovieStates));
            }
            catch (Exception ex)
            { await _pageService.DisplayAlert("Error", $"Could not change favorite state, service responded with: {ex.Message}", "Ok"); }
        }

        private async Task FetchCredits(int retryCount = 0, int delayMilliseconds = 1000)
        {
            if (_credits == null)
            {
                try
                {
                    GetMovieCreditsResult response = await _cachedSearchClient.GetMovieCredits(Movie.Id, retryCount, delayMilliseconds, fromCache: true);
                    if (response.HttpStatusCode.IsSuccessCode())
                    {
                        _credits = JsonConvert.DeserializeObject<MovieCreditsModel>(response.Json);
                        var staffMembers = _credits.ExtractStaffToDisplay(7);
                        _personDetailModelConfigurator.SetProfileImageSrc(staffMembers);

                        Staffs = staffMembers;
                    }
                    else
                        await _pageService.DisplayAlert("Error", $"Could not fetch credits information, service responded with: {response.HttpStatusCode}", "Ok");
                }
                catch (Exception ex) { await _pageService.DisplayAlert("Error", $"Could not fetch credits information, the following exception was thrown: {ex.Message}", "Ok"); }
            }
        }

        private async Task OpenPersonPage(IStaffMemberRole staffMemberRole)
        {
            try
            {
                GetPersonsDetailsResult personDetailsResponse = await _cachedSearchClient.GetPersonsDetails(staffMemberRole.Id, _settings.SearchLanguage, 0, 1000, fromCache: true);
                if (personDetailsResponse.HttpStatusCode.IsSuccessCode())
                {
                    GetPersonsDetailsModel personDetails = JsonConvert.DeserializeObject<GetPersonsDetailsModel>(personDetailsResponse.Json);

                    await _pageService.PushPersonsMovieCreditsPageAsync(personDetails);
                }
                else
                    await _pageService.DisplayAlert("Error", $"Could not fetch persons details, service responded: GetPersonDetails():{personDetailsResponse.HttpStatusCode}", "Ok");
            }
            catch (Exception ex) { await _pageService.DisplayAlert("Error", $"Could not fetch persons details, service responded with: {ex.Message}", "Ok"); }
        }
    }
}
