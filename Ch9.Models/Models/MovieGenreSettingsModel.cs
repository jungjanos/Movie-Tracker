using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Ch9.Models
{
    /// <summary>
    /// Model class serving to hold the current movie genre preferences for the user
    /// Contains list of genres and their current selection state
    /// </summary>
    public class MovieGenreSettingsModel
    {
        public ObservableCollection<GenreItem> UserGenreSelection { get; private set; }
        public HashSet<int> PreferredCategories => new HashSet<int>(UserGenreSelection.Where(x => x.IsSelected).Select(y => y.Id));

        public MovieGenreSettingsModel(ObservableCollection<GenreItem> genreSettings)
        {
            UserGenreSelection = genreSettings ?? GetDefaults();            
        }
    
        private ObservableCollection<GenreItem> GetDefaults()
        {
            var defaults = new ObservableCollection<GenreItem>();

            defaults.Add(new GenreItem { Id = 28, GenreName = "Action", IsSelected = true });
            defaults.Add(new GenreItem { Id = 12, GenreName = "Adventure", IsSelected = true });
            defaults.Add(new GenreItem { Id = 16, GenreName = "Animation", IsSelected = true });
            defaults.Add(new GenreItem { Id = 35, GenreName = "Comedy", IsSelected = true });
            defaults.Add(new GenreItem { Id = 80, GenreName = "Crime", IsSelected = true });
            defaults.Add(new GenreItem { Id = 99, GenreName = "Documentary", IsSelected = false });
            defaults.Add(new GenreItem { Id = 18, GenreName = "Drama", IsSelected = true });
            defaults.Add(new GenreItem { Id = 10751, GenreName = "Family", IsSelected = true });
            defaults.Add(new GenreItem { Id = 14, GenreName = "Fantasy", IsSelected = true });
            defaults.Add(new GenreItem { Id = 36, GenreName = "History", IsSelected = true });
            defaults.Add(new GenreItem { Id = 27, GenreName = "Horror", IsSelected = true });
            defaults.Add(new GenreItem { Id = 10402, GenreName = "Music", IsSelected = false });
            defaults.Add(new GenreItem { Id = 9648, GenreName = "Mystery", IsSelected = true });
            defaults.Add(new GenreItem { Id = 10749, GenreName = "Romance", IsSelected = true });
            defaults.Add(new GenreItem { Id = 878, GenreName = "Science Fiction", IsSelected = true });
            defaults.Add(new GenreItem { Id = 10770, GenreName = "TV Movie", IsSelected = false });
            defaults.Add(new GenreItem { Id = 53, GenreName = "Thriller", IsSelected = true });
            defaults.Add(new GenreItem { Id = 10752, GenreName = "War", IsSelected = true });
            defaults.Add(new GenreItem { Id = 37, GenreName = "Western", IsSelected = true });

            return defaults;
        }    
    }
}
