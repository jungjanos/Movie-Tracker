using Ch9.ApiClient;
using Ch9.Models;
using Ch9.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Ch9
{
    public class PersonsMovieCreditsPageViewModel : INotifyPropertyChanged
    {
        private readonly ISettings _settings;
        private readonly ITmdbCachedSearchClient _cachedSearchClient;
        private readonly IMovieDetailModelConfigurator _movieDetailModelConfigurator;
        private readonly IPageService _pageService;

        public GetPersonsMovieCreditsModel PersonsMovieCreditsModel { get; set; }

        private bool _actorOrCrewSwitch;
        public bool ActorOrCrewSwitch
        {
            get => _actorOrCrewSwitch;
            set => SetProperty(ref _actorOrCrewSwitch, value);
        }

        public ICommand OnItemTappedCommand { get; private set; }

        public PersonsMovieCreditsPageViewModel(
            GetPersonsMovieCreditsModel personsMovieCreditsModel,
            ISettings settings,
            ITmdbCachedSearchClient cachedSearchClient,
            IMovieDetailModelConfigurator movieDetailModelConfigurator,
            IPageService pageService
            )
        {

            _settings = settings;
            _cachedSearchClient = cachedSearchClient;
            _movieDetailModelConfigurator = movieDetailModelConfigurator;
            _pageService = pageService;

            _movieDetailModelConfigurator.SetImageSrc(personsMovieCreditsModel.MoviesAsActor);
            _movieDetailModelConfigurator.SetImageSrc(personsMovieCreditsModel.MoviesAsCrewMember);
            _movieDetailModelConfigurator.SetGenreNamesFromGenreIds(personsMovieCreditsModel.MoviesAsActor);
            _movieDetailModelConfigurator.SetGenreNamesFromGenreIds(personsMovieCreditsModel.MoviesAsCrewMember);

            PersonsMovieCreditsModel = personsMovieCreditsModel;
            OnItemTappedCommand = new Command<MovieDetailModel>(async mov =>
            {
                await _pageService.PushAsync(mov);
            });
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
