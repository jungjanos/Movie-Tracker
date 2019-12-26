using Ch9.Ui.Contracts.Models;
using Ch9.Services.Contracts;
using Ch9.Infrastructure.Extensions;
using Ch9.Data.Contracts;

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Ch9.Services.LocalSettings
{
    /// <summary>
    /// Service to manage the users movie genre preferences: 
    /// -fetching from device, 
    /// -saving to device, 
    /// -updating genre tags and names from server
    /// </summary>
    public class MovieGenreSettingsService : IMovieGenreSettingsService
    {
        private readonly ITmdbApiService _tmdbApiService;
        private readonly IDictionary<string, object> _appDictionary;
        private readonly IPersistLocalSettings _localSettingsPersister;

        public MovieGenreSettingsService(ITmdbApiService tmdbApiService, IDictionary<string, object> appDictionary, IPersistLocalSettings localSettingsPersister)
        {
            _tmdbApiService = tmdbApiService;
            _appDictionary = appDictionary;
            _localSettingsPersister = localSettingsPersister;
        }

        public MovieGenreSettingsModel GetGenreSetting()
        {
            ObservableCollection<GenreItem> genreItems = null;

            if (_appDictionary.ContainsKey(nameof(MovieGenreSettingsModel)))
            {
                genreItems = JsonConvert.DeserializeObject<ObservableCollection<GenreItem>>(_appDictionary[nameof(MovieGenreSettingsModel)].ToString());
            }

            return new MovieGenreSettingsModel(genreItems);
        }


        /// <summary>        
        /// updates the existing MovieGenreSettingsModel based on the collection passed in.
        /// This is necessary if new genre tags appear on the server or the user changes 
        /// the preferred language. In the later case the display names assigned to the genreids 
        /// are replaced
        /// </summary>
        /// <param name="updatedGenreIdNamePairs">up to date collection of genreid-to-name pairs</param>
        /// <param name="movieGenreSettings">object to update</param>
        private void UpdateExistingGenreCategories(GenreIdNamePair[] updatedGenreIdNamePairs, MovieGenreSettingsModel movieGenreSettings)
        {
            if (updatedGenreIdNamePairs?.Length < 1) return;

            // Delete GenreItem-s which are not included in the new update
            IEnumerable<GenreItem> toRemove = movieGenreSettings.UserGenreSelection.Where(x => updatedGenreIdNamePairs.Count(y => y.Id == x.Id) == 0);
            foreach (GenreItem item in toRemove)
                movieGenreSettings.UserGenreSelection.Remove(item);


            // Update the old elements with the new name
            foreach (var item in movieGenreSettings.UserGenreSelection)
                item.GenreName = updatedGenreIdNamePairs.First(x => x.Id == item.Id).Name;

            // Prepare and add new elements to the display collection
            var newElements = updatedGenreIdNamePairs
                .Where(x => movieGenreSettings.UserGenreSelection
                .Count(y => y.Id == x.Id) == 0)
                .Select(z =>  new GenreItem { Id = z.Id, GenreName = z.Name, IsSelected = true });

            foreach (GenreItem item in newElements)
                movieGenreSettings.UserGenreSelection.Add(item);
        }

        /// <summary>
        /// Persists the current genre settings locally on the device
        /// </summary>
        public async Task SaveGenreSelection(MovieGenreSettingsModel movieGenreSettings)
        {
            _appDictionary[nameof(MovieGenreSettingsModel)] = JsonConvert.SerializeObject(movieGenreSettings.UserGenreSelection);
            await _localSettingsPersister.SavePropertiesAsync();
        }

        /// <summary>
        /// Updates the names assigned to the genreid-s to be consistent with the selected language
        /// </summary>        
        public async Task UpdateGenreListLanguage(string newLanguage, MovieGenreSettingsModel movieGenreSettings)
        {
            var response = await _tmdbApiService.TryGetGenreIdsWithNames(newLanguage, 2, 1000);
            if (response.HttpStatusCode.IsSuccessCode())
            {
                var fetchedCategories = response.GenreIdNamePairs;
                UpdateExistingGenreCategories(fetchedCategories.Genres, movieGenreSettings);

                await SaveGenreSelection(movieGenreSettings);
            }
        }
    }
}
