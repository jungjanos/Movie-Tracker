using Ch9.ApiClient;
using Ch9.Models;
using Ch9.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Ch9
{
    public class MovieDetailPageViewModel : INotifyPropertyChanged
    {
        public MovieDetailModel Movie { get; set; }
        private readonly ISettings _settings;
        private readonly ITmdbCachedSearchClient _cachedSearchClient;
        private readonly UsersMovieListsService2 _movieListsService2;
        private readonly IMovieDetailModelConfigurator _movieDetailModelConfigurator;
        private readonly IVideoService _videoService;
        private readonly IPageService _pageService;
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

        public bool? MovieIsAlreadyOnActiveList => _movieListsService2.CustomListsService.CheckIfMovieIsOnActiveList(Movie.Id);

        private bool _movieStatesFetchFinished;
        public bool MovieStatesFetchFinished
        {
            get => _movieStatesFetchFinished;
            set => SetProperty(ref _movieStatesFetchFinished, value);
        }

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

        public MovieDetailPageViewModel(
            MovieDetailModel movie,
            ISettings settings,
            ITmdbCachedSearchClient cachedSearchClient,
            UsersMovieListsService2 movieListsService2,
            IMovieDetailModelConfigurator movieDetailModelConfigurator,
            IVideoService videoService,
            IPageService pageService)
        {
            Movie = movie;
            _settings = settings;
            _cachedSearchClient = cachedSearchClient;
            _movieListsService2 = movieListsService2;
            _movieDetailModelConfigurator = movieDetailModelConfigurator;
            _videoService = videoService;
            _pageService = pageService;
            _fetchGallery = UpdateImageCollection();
            MovieStatesFetchFinished = false;

            HomeCommand = new Command(async () => await _pageService.PopToRootAsync());
            ReviewsCommand = new Command(async () => await _pageService.PushReviewsPage(this));
            RecommendationsCommand = new Command(async () => await _pageService.PushRecommendationsPageAsync(Movie));
            ToggleWatchlistCommand = new Command(async () => await OnToggleWatchlistCommand());
            AddToListCommand = new Command(async () => await OnAddToListCommand());
            ToggleFavoriteCommand = new Command(async () => await OnToggleFavoriteCommand(), () => MovieStatesFetchFinished);
            TapImageCommand = new Command(async () =>
            {
                if (SelectedGalleryImage == null)
                    return;

                if (!SelectedGalleryImage.HasAttachedVideo)
                    await _pageService.PushLargeImagePageAsync(this);
                else
                {
                    if (SelectedGalleryImage.AttachedVideo?.Streams == null)
                        await _videoService.PopulateWithStreams(SelectedGalleryImage.AttachedVideo);

                    if (SelectedGalleryImage.AttachedVideo?.Streams?.SelectedVideoStream != null)
                        await _pageService.PushVideoPageAsync(this);
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
                await FetchMovieCreditsOfStaffMember(staffMemberRole);
            });

            DisplayImages = Movie.MovieImages;
        }

        public async Task Initialize()
        {
            _ = FetchMovieStates();
            FetchMovieDetailsResult movieDetailsResult = await _cachedSearchClient.FetchMovieDetails(Movie.Id, _settings.SearchLanguage);
            if (movieDetailsResult.HttpStatusCode.IsSuccessCode())
                JsonConvert.PopulateObject(movieDetailsResult.Json, Movie);
        }

        // starts a hot task to fetch and adjust gallery image paths as early as possible        
        private async Task UpdateImageCollection()
        {
            GalleryIsBusy = true;
            try
            {
                var moveImagesResponse = await _cachedSearchClient.GetMovieImages(Movie.Id, _settings.SearchLanguage, null, true);
                var success = moveImagesResponse.HttpStatusCode.IsSuccessCode();
                if (success)
                    await Task.Run(() => JsonConvert.PopulateObject(moveImagesResponse.Json, Movie.ImageDetailCollection));

                if (success)
                    _movieDetailModelConfigurator.SetGalleryImageSources(Movie);
            }
            catch { }
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
            if (!_settings.HasTmdbAccount)
            {
                await _pageService.DisplayAlert("Info", "You have to log in with a user account to use this function", "Ok");
                return;
            }

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
            if (!_settings.HasTmdbAccount)
            {
                await _pageService.DisplayAlert("Info", "You have to log in with a user account to use this function", "Ok");
                return;
            }

            if (MovieIsAlreadyOnActiveList == null)
            {
                await _pageService.DisplayAlert("Info", "You have to select a valid public list to be able to add movies to it", "Cancel");
                return;
            }

            try
            {
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
            if (!_settings.HasTmdbAccount)
            {
                await _pageService.DisplayAlert("Info", "You have to log in with a user account to use this function", "Ok");
                return;
            }

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

        private async Task FetchMovieStates()
        {
            GetAccountMovieStatesResult response = await _cachedSearchClient.GetAccountMovieStates(Movie.Id, guestSessionId: null, retryCount: 3, delayMilliseconds: 1000);
            if (response.HttpStatusCode.IsSuccessCode())
            {
                MovieStates = response.States;
                MovieStatesFetchFinished = true;
                RefreshCanExecutes();
            }
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
                        _movieDetailModelConfigurator.SetProfileImageSrc(staffMembers);

                        Staffs = staffMembers;
                    }
                    else
                        await _pageService.DisplayAlert("Error", $"Could not fetch credits information, service responded with: {response.HttpStatusCode}", "Ok");
                }
                catch (Exception ex) { await _pageService.DisplayAlert("Error", $"Could not fetch credits information, the following exception was thrown: {ex.Message}", "Ok"); }
            }
        }

        private async Task FetchMovieCreditsOfStaffMember(IStaffMemberRole staffMemberRole)
        {
            try
            {
                GetPersonsMovieCreditsResult response = await _cachedSearchClient.GetPersonsMovieCredits(staffMemberRole.Id, _settings.SearchLanguage, 0, 1000, fromCache: true);
                if (response.HttpStatusCode.IsSuccessCode())
                {
                    GetPersonsMovieCreditsModel personsMovieCredits = JsonConvert.DeserializeObject<GetPersonsMovieCreditsModel>(response.Json);
                    await _pageService.PushPersonsMovieCreditsPageAsync(personsMovieCredits);
                }
                else
                    await _pageService.DisplayAlert("Error", $"Could not fetch persons movie participations, service responded with: {response.HttpStatusCode}", "Ok");
            }
            catch (Exception ex) { await _pageService.DisplayAlert("Error", $"Could not fetch persons movie participations, service responded with: {ex.Message}", "Ok"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RefreshCanExecutes()
        {
            ((Command)ToggleFavoriteCommand).ChangeCanExecute();
        }

        private void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
