using Ch9.ApiClient;
using Ch9.Models;
using Ch9.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        private readonly ReviewsPageViewModel _reviewsPageViewModel;

        private ObservableCollection<ImageModel> _displayImages;
        public ObservableCollection<ImageModel> DisplayImages
        {
            get => _displayImages;
            set => SetProperty(ref _displayImages, value);
        }

        private int _galleryIndex;
        public int GalleryIndex
        {
            get => _galleryIndex;
            set => SetProperty(ref _galleryIndex, value);
        }

        // TODO : Raise Bugreport : SelectedItem property on PanCardsView does not get properly updated when the ItemSource propery changes
        // TODO: Raise bugreport : SelectedIndex property on PanCardsView does not get properly reset when the ItemSource propery changes
        // TODO :  after the PanCardsView UI Control gets bugfixed, correct this one to a "proper set-get" property
        public ImageModel SelectedGalleryImage
        {
            get
            {
                if (0 <= GalleryIndex)
                    return DisplayImages[GalleryIndex];
                else return null;
            }
        }


        private bool _galleryIsBusy;
        public bool GalleryIsBusy
        {
            get => _galleryIsBusy;
            set => SetProperty(ref _galleryIsBusy, value);
        }

        public bool? MovieIsAlreadyOnActiveList => _movieListsService2.CustomListsService.CheckIfMovieIsOnActiveList(Movie.Id);
        private bool _movieHasReviews;
        public bool MovieHasReviews
        {
            get => _movieHasReviews;
            set => SetProperty(ref _movieHasReviews, value);
        }

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

        public string Str1 { get; set; } = "https://r4---sn-c0q7lns7.googlevideo.com/videoplayback?expire=1564010909&ei=PZU4XaubL4il-ga6tq6gAQ&ip=80.98.157.94&id=o-ADTAv_oGVYw58KOMhFwEq1RN2URcIZrqsn6CyYjAaY-O&itag=22&source=youtube&requiressl=yes&mm=31%2C26&mn=sn-c0q7lns7%2Csn-4g5e6nsy&ms=au%2Conr&mv=m&mvi=3&pl=16&initcwndbps=1581250&mime=video%2Fmp4&ratebypass=yes&dur=138.483&lmt=1544418834582859&mt=1563989197&fvip=4&c=WEB&txp=5535432&sparams=expire%2Cei%2Cip%2Cid%2Citag%2Csource%2Crequiressl%2Cmime%2Cratebypass%2Cdur%2Clmt&sig=ALgxI2wwRQIhAO1XziEX9RkqaCvfUZ3ZXfv2gaKk82W9Jet2HglxVveSAiBomRyIMIYk089qL6MpGTi0_hNS2hk5jiRdpCGXA-xN-w%3D%3D&lsparams=mm%2Cmn%2Cms%2Cmv%2Cmvi%2Cpl%2Cinitcwndbps&lsig=AHylml4wRQIgSPY48XfSZ4f9KougX6UrTtYlokW2Ydr1dY0uIBRYH2ECIQDoKuhBzMK13g9dgeDJeJzcXO7BHmmLpPEQc5Ug2NKUlQ%3D%3D";

        public ICommand HomeCommand { get; private set; }
        public ICommand RecommendationsCommand { get; private set; }
        public ICommand ReviewsCommand { get; private set; }
        public ICommand ToggleWatchlistCommand { get; private set; }
        public ICommand AddToListCommand { get; private set; }
        public ICommand ToggleFavoriteCommand { get; private set; }
        public ICommand TapImageCommand { get; private set; }
        public ICommand ChangeDisplayedImageTypeCommand { get; private set; }

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
            MovieHasReviews = false;
            MovieStatesFetchFinished = false;
            _reviewsPageViewModel = new ReviewsPageViewModel(this, _cachedSearchClient);

            HomeCommand = new Command(async () => await _pageService.PopToRootAsync());
            ReviewsCommand = new Command(async () => await _pageService.PushAsync(_reviewsPageViewModel), () => MovieStatesFetchFinished);
            RecommendationsCommand = new Command(async () => await _pageService.PushRecommendationsPageAsync(Movie));
            ToggleWatchlistCommand = new Command(async () => await OnToggleWatchlistCommand());
            AddToListCommand = new Command(async () => await OnAddToListCommand());
            ToggleFavoriteCommand = new Command(async () => await OnToggleFavoriteCommand(), () => MovieStatesFetchFinished);
            TapImageCommand = new Command(async () =>
            {
                if (GalleryIndex < 0)
                    return;                

                if (!SelectedGalleryImage.HasAttachedVideo)
                    await _pageService.PushLargeImagePageAsync(this);
                else 
                {
                    if (SelectedGalleryImage.AttachedVideo?.Streams == null)                    
                        await _videoService.PopulateWithStreams(DisplayImages[GalleryIndex].AttachedVideo);
                    
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
                GalleryIsBusy = false;
                ((Command)ChangeDisplayedImageTypeCommand).ChangeCanExecute();

            }, () => !GalleryIsBusy);

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

        // TODO : property should set be with awaiter from UI thread!!!
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

        public event PropertyChangedEventHandler PropertyChanged;

        private void RefreshCanExecutes()
        {
            ((Command)ReviewsCommand).ChangeCanExecute();
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
