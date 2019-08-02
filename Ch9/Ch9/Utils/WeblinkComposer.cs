using Ch9.Models;

namespace Ch9.Utils
{
    public class WeblinkComposer
    {
        private const string IMDb_Base = "https://www.imdb.com";
        private const string IMDb_Person_Base = "https://www.imdb.com/name/";
        private const string IMDb_Movie_Base = "https://www.imdb.com/title/";

        private const string TMDb_Base = "https://www.themoviedb.org";
        private const string TMDb_Person_Base = "https://www.themoviedb.org/person/";
        private const string TMDb_Movie_Base = "https://www.themoviedb.org/movie/";

        private readonly ISettings _settings;


        public WeblinkComposer(ISettings settings)
        {
            _settings = settings;
        }

        public string Compose(GetPersonsDetailsModel person)
        {
            if (_settings.InfoLinkTargetHomePage == InformationLinkTargetHomePage.IMDb)
            {
                if (!string.IsNullOrEmpty(person.ImdbId))
                    return IMDb_Person_Base + person.ImdbId;
            }
            else if (_settings.InfoLinkTargetHomePage == InformationLinkTargetHomePage.TMDb)
                return TMDb_Person_Base + person.Id;
            else if (_settings.InfoLinkTargetHomePage == InformationLinkTargetHomePage.Invalid)
                return null;

            return null;
        }

        public string Compose(MovieDetailModel movie)
        {
            if (_settings.InfoLinkTargetHomePage == InformationLinkTargetHomePage.IMDb)
            {
                if (!string.IsNullOrEmpty(movie.ImdbId))
                    return IMDb_Movie_Base + movie.ImdbId;
            }
            else if (_settings.InfoLinkTargetHomePage == InformationLinkTargetHomePage.TMDb)
                return TMDb_Movie_Base + movie.Id;
            else if (_settings.InfoLinkTargetHomePage == InformationLinkTargetHomePage.Invalid)
                return null;

            return null;
        }
    }
}
