using Ch9.ApiClient;
using Ch9.Models;
using Ch9.Services;
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

namespace Ch9.ViewModels
{
    public class PersonsMovieCreditsPageViewModel : INotifyPropertyChanged
    {
        private readonly ISettings _settings;
        private readonly ITmdbCachedSearchClient _cachedSearchClient;
        private readonly IMovieDetailModelConfigurator _movieDetailModelConfigurator;
        private readonly IPersonDetailModelConfigurator _personDetailModelConfigurator;
        private readonly WeblinkComposer _weblinkComposer;
        private readonly IPageService _pageService;
        private readonly Task _fetchGalleryImages;

        public GetPersonsDetailsModel PersonsDetails { get; private set; }
        public GetPersonsMovieCreditsModel PersonsMovieCreditsModel { get; private set; }
        public ObservableCollection<ImageModel> DisplayImages { get; private set; }
        public int NumberOfMovies => (PersonsMovieCreditsModel.MoviesAsActor?.Length ?? 0) + (PersonsMovieCreditsModel.MoviesAsCrewMember?.Length ?? 0);

        private bool _actorOrCrewSwitch;
        public bool ActorOrCrewSwitch
        {
            get => _actorOrCrewSwitch;
            set => SetProperty(ref _actorOrCrewSwitch, value);
        }

        public ICommand OnItemTappedCommand { get; private set; }
        public ICommand OpenWeblinkCommand { get; private set; }
        public ICommand OpenInfolinkCommand { get; private set; }

        public PersonsMovieCreditsPageViewModel(
            GetPersonsDetailsModel personDetails,
            GetPersonsMovieCreditsModel personsMovieCreditsModel,
            ISettings settings,
            ITmdbCachedSearchClient cachedSearchClient,
            IMovieDetailModelConfigurator movieDetailModelConfigurator,
            IPersonDetailModelConfigurator personDetailModelConfigurator,
            WeblinkComposer weblinkComposer,
            IPageService pageService
            )
        {

            _settings = settings;
            _cachedSearchClient = cachedSearchClient;
            _movieDetailModelConfigurator = movieDetailModelConfigurator;
            _personDetailModelConfigurator = personDetailModelConfigurator;
            _weblinkComposer = weblinkComposer;
            _pageService = pageService;

            _movieDetailModelConfigurator.SetImageSrc(personsMovieCreditsModel.MoviesAsActor);
            _movieDetailModelConfigurator.SetImageSrc(personsMovieCreditsModel.MoviesAsCrewMember);
            _movieDetailModelConfigurator.SetGenreNamesFromGenreIds(personsMovieCreditsModel.MoviesAsActor);
            _movieDetailModelConfigurator.SetGenreNamesFromGenreIds(personsMovieCreditsModel.MoviesAsCrewMember);

            PersonsDetails = personDetails;
            personsMovieCreditsModel.SortMoviesByYearDesc();
            PersonsMovieCreditsModel = personsMovieCreditsModel;

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

            _fetchGalleryImages = UpdateImageCollection();
        }

        private async Task UpdateImageCollection()
        {
            try
            {
                GetPersonImagesResult response = await _cachedSearchClient.GetPersonImages(PersonsDetails.Id, retryCount: 1, delayMilliseconds: 1000, fromCache: true);
                if (response.HttpStatusCode.IsSuccessCode())
                {
                    ImageModel[] imagesFetched = null;
                    await Task.Run(() =>
                    {
                        imagesFetched = JsonConvert.DeserializeObject<ImageDetailCollection>(response.Json).Profiles;
                        _personDetailModelConfigurator.SetProfileGalleryImageSources(imagesFetched);
                    });

                    var first = DisplayImages.FirstOrDefault();

                    foreach (var image in imagesFetched)
                        if (image.FilePath != first?.FilePath)
                            DisplayImages.Add(image);
                }
            }
            catch (Exception ex) { await _pageService.DisplayAlert("Error", $"Could not load gallery, service responded with: {ex.Message}", "Ok"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName]string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

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
