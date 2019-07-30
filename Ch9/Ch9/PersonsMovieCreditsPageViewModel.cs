using Ch9.ApiClient;
using Ch9.Models;
using Ch9.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Ch9
{
    public class PersonsMovieCreditsPageViewModel : INotifyPropertyChanged
    {
        private readonly ISettings _settings;
        private readonly ITmdbCachedSearchClient _cachedSearchClient;
        private readonly IMovieDetailModelConfigurator _movieDetailModelConfigurator;
        private readonly IPersonDetailModelConfigurator _personDetailModelConfigurator;
        private readonly IPageService _pageService;

        public GetPersonsDetailsModel PersonsDetails { get; private set; }
        public GetPersonsMovieCreditsModel PersonsMovieCreditsModel { get; private set; }

        public ObservableCollection<ImageModel> DisplayImages { get; private set; }

        private bool _actorOrCrewSwitch;
        public bool ActorOrCrewSwitch
        {
            get => _actorOrCrewSwitch;
            set => SetProperty(ref _actorOrCrewSwitch, value);
        }

        public ICommand OnItemTappedCommand { get; private set; }
        public ICommand OpenWeblinkCommand { get; private set; }

        public PersonsMovieCreditsPageViewModel(
            GetPersonsDetailsModel personDetails,
            GetPersonsMovieCreditsModel personsMovieCreditsModel,
            ISettings settings,
            ITmdbCachedSearchClient cachedSearchClient,
            IMovieDetailModelConfigurator movieDetailModelConfigurator,
            IPersonDetailModelConfigurator personDetailModelConfigurator,
            IPageService pageService
            )
        {

            _settings = settings;
            _cachedSearchClient = cachedSearchClient;
            _movieDetailModelConfigurator = movieDetailModelConfigurator;
            _personDetailModelConfigurator = personDetailModelConfigurator;
            _pageService = pageService;

            _movieDetailModelConfigurator.SetImageSrc(personsMovieCreditsModel.MoviesAsActor);
            _movieDetailModelConfigurator.SetImageSrc(personsMovieCreditsModel.MoviesAsCrewMember);
            _movieDetailModelConfigurator.SetGenreNamesFromGenreIds(personsMovieCreditsModel.MoviesAsActor);
            _movieDetailModelConfigurator.SetGenreNamesFromGenreIds(personsMovieCreditsModel.MoviesAsCrewMember);

            PersonsDetails = personDetails;
            PersonsMovieCreditsModel = personsMovieCreditsModel;

            var firstImage = new ImageModel();
            _personDetailModelConfigurator.SetProfileGalleryPictureImageSrc(firstImage, PersonsDetails);

            DisplayImages = new ObservableCollection<ImageModel>(new ImageModel[] { firstImage });

            OnItemTappedCommand = new Command<MovieDetailModel>(async mov => await _pageService.PushAsync(mov));

            OpenWeblinkCommand = new Command<string>(url => _pageService.OpenWeblink(url));
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
