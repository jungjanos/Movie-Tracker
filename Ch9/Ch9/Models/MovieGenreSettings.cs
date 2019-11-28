﻿using Ch9.Ui.Contracts.Models;
using Ch9.Services.Contracts;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Ch9.Infrastructure.Extensions;

namespace Ch9.Models
{
    //TODO : extract interface
    // contains the movie genre preferences of the user to filter the results of WebAPI queries
    public class MovieGenreSettings
    {
        private ITmdbApiService _tmdbApiService;
        private IDictionary<string, object> _appDictionary;
        public ObservableCollection<GenreItem> UserGenreSelection { get; set; }

        public HashSet<int> PreferredCategories => new HashSet<int>(UserGenreSelection.Where(x => x.IsSelected).Select(y => y.Id));

        public MovieGenreSettings(ITmdbApiService tmdbApiService, IDictionary<string, object> appDictionary)
        {
            _tmdbApiService = tmdbApiService;
            _appDictionary = appDictionary;
            InitializeGenreSelectionDisplay();
        }

        // This runs at Application startup (constructor)
        // fetches a previously persisted GenreSelection 
        // or creates a new default one
        private void InitializeGenreSelectionDisplay()
        {
            if (_appDictionary.ContainsKey(nameof(UserGenreSelection)))
                UserGenreSelection = JsonConvert.DeserializeObject<ObservableCollection<GenreItem>>(_appDictionary[nameof(UserGenreSelection)].ToString());
            else
            {
                UserGenreSelection = new ObservableCollection<GenreItem>();
                UserGenreSelection.Add(new GenreItem { Id = 28, GenreName = "Action", IsSelected = true });
                UserGenreSelection.Add(new GenreItem { Id = 12, GenreName = "Adventure", IsSelected = true });
                UserGenreSelection.Add(new GenreItem { Id = 16, GenreName = "Animation", IsSelected = true });
                UserGenreSelection.Add(new GenreItem { Id = 35, GenreName = "Comedy", IsSelected = true });
                UserGenreSelection.Add(new GenreItem { Id = 80, GenreName = "Crime", IsSelected = true });
                UserGenreSelection.Add(new GenreItem { Id = 99, GenreName = "Documentary", IsSelected = false });
                UserGenreSelection.Add(new GenreItem { Id = 18, GenreName = "Drama", IsSelected = true });
                UserGenreSelection.Add(new GenreItem { Id = 10751, GenreName = "Family", IsSelected = true });
                UserGenreSelection.Add(new GenreItem { Id = 14, GenreName = "Fantasy", IsSelected = true });
                UserGenreSelection.Add(new GenreItem { Id = 36, GenreName = "History", IsSelected = true });
                UserGenreSelection.Add(new GenreItem { Id = 27, GenreName = "Horror", IsSelected = true });
                UserGenreSelection.Add(new GenreItem { Id = 10402, GenreName = "Music", IsSelected = false });
                UserGenreSelection.Add(new GenreItem { Id = 9648, GenreName = "Mystery", IsSelected = true });
                UserGenreSelection.Add(new GenreItem { Id = 10749, GenreName = "Romance", IsSelected = true });
                UserGenreSelection.Add(new GenreItem { Id = 878, GenreName = "Science Fiction", IsSelected = true });
                UserGenreSelection.Add(new GenreItem { Id = 10770, GenreName = "TV Movie", IsSelected = false });
                UserGenreSelection.Add(new GenreItem { Id = 53, GenreName = "Thriller", IsSelected = true });
                UserGenreSelection.Add(new GenreItem { Id = 10752, GenreName = "War", IsSelected = true });
                UserGenreSelection.Add(new GenreItem { Id = 37, GenreName = "Western", IsSelected = true });
            }

            foreach (GenreItem item in UserGenreSelection)
                item.PropertyChanged += GenreItem_PropertyChanged;
        }

        // updates the existing categories with the new categories passed as attribute
        private void UpdateExistingGenreCategories(GenreIdNamePair[] genreIdNamePairs)
        {
            if (genreIdNamePairs?.Length < 1) return;

            // Delete GenreItem-s which are not included in the new update
            IEnumerable<GenreItem> toRemove = UserGenreSelection.Where(x => genreIdNamePairs.Count(y => y.Id == x.Id) == 0);
            foreach (GenreItem item in toRemove)
                UserGenreSelection.Remove(item);


            // Update the old elements with the new name
            foreach (var item in UserGenreSelection)
            {
                item.GenreName = genreIdNamePairs.First(x => x.Id == item.Id).Name;
            }

            // Prepare and add new elements to the display collection
            var newElements = genreIdNamePairs
                .Where(x => UserGenreSelection
                .Count(y => y.Id == x.Id) == 0)
                .Select(z =>
                {
                    var i = new GenreItem { Id = z.Id, GenreName = z.Name, IsSelected = true };
                    i.PropertyChanged += GenreItem_PropertyChanged;
                    return i;
                });

            foreach (GenreItem item in newElements)
                UserGenreSelection.Add(item);
        }

        private void GenreItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SaveGenreSelectionDisplay();
        }

        public void SaveGenreSelectionDisplay()
        {
            _appDictionary[nameof(UserGenreSelection)] = JsonConvert.SerializeObject(UserGenreSelection);
        }

        public async Task OnSearchLanguageChanged(string newLanguage)
        {
            var response = await _tmdbApiService.TryGetGenreIdsWithNames(newLanguage, 2, 1000);
            if (response.HttpStatusCode.IsSuccessCode())
            {
                var fetchedCategories = response.GenreIdNamePairs;
                UpdateExistingGenreCategories(fetchedCategories.Genres);
            }
        }
    }
}
