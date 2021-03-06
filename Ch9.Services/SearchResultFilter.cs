﻿using Ch9.Models;
using Ch9.Services.Contracts;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Ch9.Services
{
    // Aims to provide filtering of the TMDB WebAPI response based on user 
    // movie preferences (genre setting, timeframe) 
    // Adult filter is implemented on server side
    public class SearchResultFilter : ISearchResultFilter
    {
        private readonly ISettings settings;
        private readonly MovieGenreSettingsModel _genreSettings;


        private Func<IEnumerable<MovieDetailModel>, IEnumerable<MovieDetailModel>> filterByTimeframe;
        private Func<IEnumerable<MovieDetailModel>, IEnumerable<MovieDetailModel>> filterByGenres;

        public SearchResultFilter(ISettings settings, MovieGenreSettingsModel genreSettings)
        {
            this.settings = settings;
            _genreSettings = genreSettings;

            filterByTimeframe = _filterByTimeframe;
            filterByGenres = _filterByGenres;
        }

        public IEnumerable<MovieDetailModel> FilterBySearchSettings(IEnumerable<MovieDetailModel> movies)
        {
            var first = filterByTimeframe(movies);
            var second = filterByGenres(first);

            return second;
        }
        public IEnumerable<MovieDetailModel> FilterForAdultOnly(IEnumerable<MovieDetailModel> movies)
        {
            var first = filterByTimeframe(movies);
            var second = first.Where(movie => movie.Adult == true);

            return second;
        }

        public IEnumerable<MovieDetailModel> FilterBySearchSettingsIncludeAdult(IEnumerable<MovieDetailModel> movies)
        {
            var first = filterByTimeframe(movies);
            var second = first.Where(movie => movie.Adult || _genreSettings.PreferredCategories.Overlaps(movie.GenreIds));

            return second;
        }

        // Filters out movies which have a release date in the present
        // Filters out movies which have a release year older than allowed
        // expected behaviour: shouldn't count fractions of years 
        private IEnumerable<MovieDetailModel> _filterByTimeframe(IEnumerable<MovieDetailModel> movies)
        {
            DateTime present = DateTime.Now;
            int thisYear = DateTime.Now.Year;
            int oldestAllowed = thisYear - settings.SearchPeriod;

            return movies.Where(x => (oldestAllowed <= x?.ReleaseDate?.Year) && (x?.ReleaseDate <= present));
        }

        // Filtrs out movies which dont have at least one genre (e.g. crime, thriller, fantasy, etc)
        // common with user's genre preferences. 
        private IEnumerable<MovieDetailModel> _filterByGenres(IEnumerable<MovieDetailModel> movies)
        {
            return movies.Where(x => _genreSettings.PreferredCategories.Overlaps(x.GenreIds));
        }
    }
}
